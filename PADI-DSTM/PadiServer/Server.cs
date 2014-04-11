using DSTMLIB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Timers;

namespace PADIServer
{
    class ServerRunner
    {
        static void Main(string[] args)
        {
            KeyValuePair<int, int> idAndPort;
            System.Console.WriteLine("Bootstrapping...");
            TcpChannel channel = new TcpChannel();
            ChannelServices.RegisterChannel(channel, false);
            System.Console.WriteLine("Registered Channel @random");

			string address = System.IO.File.ReadAllText(@"../../../mServerLocation.dat");

            MasterInterface mServer = (MasterInterface)Activator.GetObject(typeof(MasterInterface), address);
            idAndPort = mServer.RegisterTransactionalServer(getIP());

            System.Console.WriteLine("Registered at Master");
            ChannelServices.UnregisterChannel(channel);
            System.Console.WriteLine("Unbinding old port");

            BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            provider.TypeFilterLevel = TypeFilterLevel.Full;
            IDictionary props = new Hashtable();
            props["port"] = idAndPort.Value;

            channel = new TcpChannel(props, null, provider);

            ChannelServices.RegisterChannel(channel, false);
            System.Console.WriteLine("Registered Channel @" + idAndPort.Value);
            TransactionalServer ts = new TransactionalServer(idAndPort.Key, mServer, "tcp://"+getIP()+":"+idAndPort.Value+"/Server");
            RemotingServices.Marshal(ts, "Server", typeof(TransactionalServer));
            System.Console.WriteLine("SERVER ON");
            System.Console.WriteLine("Name: " + idAndPort.Key + " Port: " + idAndPort.Value + "IP: "+ getIP());
            System.Console.ReadLine();
        }

        private static string getIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (!(ip.ToString().Contains("192.168")) && ip.AddressFamily.ToString() == "InterNetwork")
                {
                    localIP = ip.ToString();
                    Console.WriteLine(localIP);
                }
            }
            return localIP;
        }
    }

    class TransactionalServer : MarshalByRefObject, ServerInterface, ParticipantInterface, CoordinatorInterface
    {
        // a collection of all the padints a server holds;
        // the correspondence is PADInt uid -> PADInt;
        // though the PADInt knows its own uid, this improves access speed
        private Dictionary<int, PADInt> _padints;
        // list of client requests (access, create, etc) that haven't been executed;
        // this happens when a server is frozen
        private Dictionary<MethodInfo, List<Object>> _pendingRequests;
        // true if the server is functioning correctly; false otherwise
        private bool _status { get; set; }
        // true if the server is simulating a fail situation; false otherwise
        private bool _fail { get; set; }
        // true if the server is simulating a freeze situation; false otherwise
        private bool _freeze { get; set; }
		// event handler to send "I'm alive" messages
        private Timer _alive;
		// time interval to send the master an "I'm Alive" message
        private const long TIMEOUT = 2000;
		// the master's interface, through which the server communicates
        private MasterInterface _master;
		// the server's id, given by the master
		private int _id;
		//the location of the server (tcp://<ip>:<port>/Server)
        private string _url;
        // map between transaction ID and a list of PADInts
        private Dictionary<int, List<int>> _transactions;
        // map between PADInt ID and the locking status
        private Dictionary<int, bool> _locks;
        
        
        // Coordinator attributes 
        // map between transaction ID and the list of servers involved
        private bool onTime;
        private bool coordinating;
        private List<bool> votes = new List<bool>();

        public TransactionalServer(int id, MasterInterface master, string url)
        {
            _id = id;
            _padints = new Dictionary<int, PADInt>();
            _transactions = new Dictionary<int, List<int>>();
            _locks = new Dictionary<int, bool>();
			_pendingRequests = new Dictionary<MethodInfo, List<Object>>();
            _alive = new Timer(TIMEOUT);
            _alive.Elapsed += IsAlive;
            _alive.Enabled = true;
            _master = master;
            _url = url;
        }

        // Is Alive Method

        void IsAlive(object sender, ElapsedEventArgs e)
        {
            _master.ImAlive(_id);
        }

        // ------------------------------------------------------------------------------------------------------------------
        // PADInt Manipulation Methods --------------------------------------------------------------------------------------

        public PADInt CreatePADInt(int uid, string server)
        {
            if (_padints.ContainsKey(uid))
                throw new TxException("SERVER: PADInt with uid " + uid + " already exists!");

            PADInt p = new PADInt(uid, server);
            Console.WriteLine("SERVER: Created PADInt with uid: " + p.UID);

            _padints.Add(uid, p);
            Console.WriteLine("Added to dictionary");

			return p;
        }

        public PADInt AccessPADInt(int uid)
        {
            if (!_padints.ContainsKey(uid))
                throw new TxException("SERVER: PADInt with uid " + uid + " doesn't exist!");

            PADInt p = _padints[uid];
            Console.WriteLine("SERVER: Accessing PADInt with uid " + uid + "...");

            return p;
        }

        // ---------------------------------------------------------------------------------------------------------------------
        // Status methods ------------------------------------------------------------------------------------------------------

        public bool Status()
        {
            return _status;
        }

		/// <summary>
		/// Simulates a server crash
		/// </summary>
		/// <returns></returns>
        public bool Fail()
        {
            _fail = true;
            _status = false;

            return _fail;
        }

		/// <summary>
		/// Simulates a non-responsive server
		/// </summary>
		/// <returns></returns>
        public bool Freeze()
        {
            _freeze = true;
            _status = false;

            return _freeze;
        }

		/// <summary>
		/// Recovers from a Freeze or Fail state
		/// </summary>
		/// <returns></returns>
        public bool Recover()
        {
            bool ok = false;

            if (_fail || _freeze)
            {
				if (_freeze)
					ok = DispatchPendindRequests();

                _fail = false;
                _freeze = false;
				_status = true;                
            }
            return ok;
        }

        private bool DispatchPendindRequests()
        {
            try
            {
                foreach (KeyValuePair<MethodInfo, List<Object>> request in _pendingRequests)
				{
					request.Key.Invoke(this, request.Value.ToArray());
				}
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }

        // -------------------------------------------------------------------------------------------------------------------
        // Participant Methods ------------------------------------------------------------------------------------------------

		public bool DoCommit(int tId, string coordinator)
		{
			foreach (int p in _transactions[tId])
			{
                this.UnlockPADInt(tId, p);
			
			}
            _transactions.Remove(tId);
			return true;
		}

		public void DoAbort(int tId, string coordinator)
		{

            foreach (int id in _transactions[tId])
            {
                _padints[id].rollback();
                UnlockPADInt(tId, id);
            }
            _transactions.Remove(tId);

		}

        public void Prepare(int tID, string coordinator, int timestamp)
        {
			bool reply = true;
			_transactions[tID].ForEach((int id) => reply = reply && _padints[id].persistValue(timestamp));
			SendVote(reply, coordinator);
        }

        private void SendVote(bool reply, string coordinator)
        {
            CoordinatorInterface coord = (CoordinatorInterface)Activator.GetObject(typeof(CoordinatorInterface), coordinator);
            coord.ReceiveVote(reply);
        }

        // -------------------------------------------------------------------------------------------------------------
        // Locking Methods

        public void LockPADInt(int transactionId, int uid ,int timestamp)
		{
			foreach (KeyValuePair<int, List<int>> t in _transactions)
			{
                if (t.Value.Contains(uid))
                {
                    throw new TxException("The PADInt" + uid + " is already locked!");
                }
			}
            if (_padints[uid].Timestamp > timestamp)
            {
                throw new TxException("The client timestamp is lower than the object's timestamp!");
            }
            else
            {
                if (!_transactions.ContainsKey(transactionId)) {
                    _transactions.Add(transactionId, new List<int>());
                    _transactions[transactionId].Add(uid);
                }else
                    _transactions[transactionId].Add(uid);
            }
        }

        public void UnlockPADInt(int transactionId, int uid)
        {
            if (_transactions[transactionId].Contains(uid))
                _transactions[transactionId].Remove(uid);
            else
                throw new TxException("The PADInt" + uid + "is not locked");
        }

        //-------------------------------------------------------------------------------------------------------------
        // Coordinator Methods ----------------------------------------------------------------------------------------

        public void ReceiveVote(bool vote)
        {
            votes.Add(vote);
        }

        public string GetServerUrl()
		{
            return _url;
        }

        public bool TxCommit(int tId, List<string> participants, int timestamp)
        {
            bool canCommit = true;
            Timer timeout = new Timer(10000);
            timeout.Elapsed += timeout_Elapsed;
            votes = new List<bool>();

            //acquire participant objects
            onTime = true;
            coordinating = true;
            List<ParticipantInterface> _serversToCommit = new List<ParticipantInterface>();
            foreach (string participant in participants)
                _serversToCommit.Add((ParticipantInterface)Activator.GetObject(typeof(ParticipantInterface), participant));

            //envia prepare
            foreach (ParticipantInterface server in _serversToCommit)
                server.Prepare(tId, _url, timestamp);
            timeout.Enabled = true;

           while (onTime)
           {
               votes.ForEach((bool x) => canCommit = (x && canCommit));
               if (canCommit)
                   break;
           }

           if (onTime && canCommit)
               _serversToCommit.ForEach((ParticipantInterface p) => p.DoCommit(tId, _url));
           else
               _serversToCommit.ForEach((ParticipantInterface p) => p.DoAbort(tId, _url));

           return canCommit;
        }

        void timeout_Elapsed(object sender, ElapsedEventArgs e)
        {
            onTime = false;
        }

        public bool TxAbort(int tId, List<string> participants)
        {
            List<ParticipantInterface> _serversToCommit = new List<ParticipantInterface>();
            ParticipantInterface p;
            foreach (string participant in participants)
            {
                p = (ParticipantInterface)Activator.GetObject(typeof(ParticipantInterface), participant);
                p.DoAbort(tId, _url);
            }
            return true;
        }

		public bool Freeze_f()
		{
			return _freeze;
		}

		public bool Fail_f()
		{
			return _fail;
		}

		public void AddPendingRequest(MethodInfo methodInfo, List<Object> parameters)
		{
			_pendingRequests.Add(methodInfo, parameters);
			return;
		}
    }
}
