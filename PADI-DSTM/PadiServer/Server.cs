﻿using DSTMLIB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
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
			RemotingConfiguration.Configure(@"../../App.config", true);

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
            System.Console.WriteLine("Server: " + idAndPort.Key + " Port: " + idAndPort.Value + " IP: "+ getIP());
            System.Console.ReadLine();
        }

        private static string getIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
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
		private bool _status;
        // true if the server is simulating a fail situation; false otherwise
        private bool _fail;
        // true if the server is simulating a freeze situation; false otherwise
		private bool _freeze;
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
		// true if server is currently inside a transaction
		// the coordinator's url for the current transaction
		private string currentCoordinator { get; set; }
		// the coordinator's url for the current transaction
		private int currentTID { get; set; }


        // Coordinator attributes 
        // map between transaction ID and the list of servers involved
        private bool onTime;
        private List<bool> votes = new List<bool>();
        private bool _prepared;

        public TransactionalServer(int id, MasterInterface master, string url)
        {
            _id = id;
            _padints = new Dictionary<int, PADInt>();
            _transactions = new Dictionary<int, List<int>>();
            _pendingRequests = new Dictionary<MethodInfo, List<Object>>();
            _alive = new Timer(TIMEOUT);
            _alive.Elapsed += IsAlive;
            _alive.Enabled = true;
            _master = master;
            _url = url;
            _prepared = false;
            _status = true;
        }

        // Is Alive Method

        void IsAlive(object sender, ElapsedEventArgs e)
        {
			if (_fail || _freeze)
				return;
            _master.ImAlive(_id, _url);
        }

		// -----------------------------------------------------------------------------------------
        // PADInt Manipulation Methods -------------------------------------------------------------

        public PADInt CreatePADInt(int uid, List<string> servers, int transactionId)
		{
			List<object> _parameters = new List<object>();
			_parameters.Add(uid);
			_parameters.Add(servers);
			_parameters.Add(transactionId);

			if (!Fail_f())
			{
				if (!Freeze_f())
				{
					Console.WriteLine("SERVER: Create request for the PADInt " + uid + " with the transaction id " + transactionId);

					if (_padints.ContainsKey(uid))
						return null;

					return CreateNewPADInt(uid, servers, transactionId);
				}
				else
				{
					AddPendingRequest(this.GetType().GetMethod("CreateNewPADInt"), _parameters);
					return null;
				}
			}
			else
				return null;
        }

		private PADInt CreateNewPADInt(int uid, List<string> servers, int transactionId)
		{
			PADInt p = new PADInt(uid, servers);

			_padints.Add(uid, p);

			if (!_transactions.ContainsKey(transactionId))
			{
				_transactions.Add(transactionId, new List<int>());
				_transactions[transactionId].Add(uid);
			}
			else
				_transactions[transactionId].Add(uid);

			return p;
		}

        public PADInt AccessPADInt(int uid, int transactionId)
        {
			List<object> _parameters = new List<object>();
			_parameters.Add(uid);
			_parameters.Add(transactionId);

			if (!Fail_f())
			{
				if (!Freeze_f())
				{
					Console.WriteLine("SERVER: Access request for the PADInt " + uid + " from the transaction " + transactionId);

					if (!_padints.ContainsKey(uid))
						return null;

					return AccessExistingPADInt(uid, transactionId);
				}
				else
				{
					AddPendingRequest(this.GetType().GetMethod("AccessExistingPADInt"), _parameters);
					return null;
				}
			}
			else
				return null;
        }

		private PADInt AccessExistingPADInt(int uid, int transactionId)
		{
			if (!_transactions.ContainsKey(transactionId))
			{
				_transactions.Add(transactionId, new List<int>());
				_transactions[transactionId].Add(uid);
			}
			else
			{
				if (!_transactions[transactionId].Contains(uid))
					_transactions[transactionId].Add(uid);
			}
			return _padints[uid];
		}

        // -----------------------------------------------------------------------------------------
        // Status methods --------------------------------------------------------------------------

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
			if (_fail || _freeze)
            {
                if (_freeze)
                    DispatchPendindRequests();

                _fail = false;
                _freeze = false;
                _status = true;
            }
            return true;
        }

        private void DispatchPendindRequests()
        {
            try
            {
                foreach (KeyValuePair<MethodInfo, List<object>> request in _pendingRequests)
                    request.Key.Invoke(this, request.Value.ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // -----------------------------------------------------------------------------------------
        // Participant Methods ---------------------------------------------------------------------

        public bool DoCommit(int tId, string coordinator)
        {
            _transactions[tId].Clear();
            _transactions.Remove(tId);
			_pendingRequests.Clear();
            _prepared = false;
            return true;
        }

        public void DoAbort(int tId)
        {
            _transactions[tId].Clear();
            _transactions.Remove(tId);
			_pendingRequests.Clear();
            _prepared = false;
        }

        public void DoRollback(int tId)
        {
            foreach (int id in _transactions[tId])
                _padints[id].Rollback();
        }

        public void Prepare(int tID, string coordinator, int timestamp)
        {
			bool reply = false;
			if ((!Fail_f()) && (!Freeze_f()))
			{
				reply = true;

				Console.WriteLine("Preparing Transaction");
				currentTID = tID;
				currentCoordinator = coordinator;
				_transactions[tID].ForEach((int id) => reply = reply && _padints[id].PersistValue(tID, timestamp));
				Console.WriteLine("Vote: " + reply);
				_prepared = true;
				SendVote(reply, coordinator);
			}
			else
			{
				SendVote(reply, coordinator);
				Console.WriteLine("Vote: " + reply);
			}
        }

        private void SendVote(bool reply, string coordinator)
        {
            CoordinatorInterface coord = (CoordinatorInterface)Activator.GetObject(typeof(CoordinatorInterface), coordinator);
            coord.ReceiveVote(reply);
        }


        //------------------------------------------------------------------------------------------
        // Coordinator Methods ---------------------------------------------------------------------

        public void ReceiveVote(bool vote)
        {
            votes.Add(vote);
        }

        public bool TxCommit(int tId, List<string> _references, int timestamp)
        {
            bool canCommit = true;
            Timer timeout = new Timer(10000);
            timeout.Elapsed += timeout_Elapsed;
            votes = new List<bool>();

            //acquire participant objects
            onTime = true;
            List<ParticipantInterface> _serversToCommit = new List<ParticipantInterface>();

            foreach (String server in _references)
                _serversToCommit.Add((ParticipantInterface)Activator.GetObject(typeof(ParticipantInterface), server));

            //send prepare
            foreach (ParticipantInterface server in _serversToCommit)
                server.Prepare(tId, _url, timestamp);

            timeout.Enabled = true;

            while (onTime)
            {
                votes.ForEach((bool x) => canCommit = (x && canCommit));
                if ((votes.Count == _serversToCommit.Count) && canCommit)
                    break;
            }

            if (onTime && canCommit)
                _serversToCommit.ForEach((ParticipantInterface p) => p.DoCommit(tId, _url));
            else
                foreach (ParticipantInterface p in _serversToCommit)
                {
                    p.DoRollback(tId);
                    p.DoAbort(tId);
                }

            timeout.Enabled = false;
            return canCommit && onTime;
        }

        void timeout_Elapsed(object sender, ElapsedEventArgs e)
        {
            onTime = false;
        }

        public bool TxAbort(int tId, List<string> participants)
        {
            List<ParticipantInterface> _serversToCommit = new List<ParticipantInterface>();
			if (_prepared)
			{
				foreach (string participant in participants)
				{
					ParticipantInterface p = (ParticipantInterface)Activator.GetObject(typeof(ParticipantInterface), participant);
					p.DoAbort(tId);
				}
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
		}
	}
}
