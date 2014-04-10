using DSTMLib;
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

            MasterInterface mServer = (MasterInterface)Activator.GetObject(typeof(MasterInterface), "tcp://localhost:8087/Server");
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
            TransactionalServer ts = new TransactionalServer(idAndPort.Key, mServer);
            RemotingServices.Marshal(ts, "TransactionalServer", typeof(TransactionalServer));
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(TransactionalServer), "Server", WellKnownObjectMode.Singleton);
            System.Console.WriteLine("SERVER ON");
            System.Console.WriteLine("Name: " + idAndPort.Key + " Port: " + idAndPort.Value);
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
        // a list of all the padints involved in a given transaction
        private List<int> _padintsTx;
        // list of client requests (access, create, etc) that haven't been executed;
        // this happens when a server is frozen
        private List<MethodBase> _pendingRequests;
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

        private string url;

        public TransactionalServer(int id, MasterInterface master)
        {
            _id = id;
            _padints = new Dictionary<int, PADInt>();
            _padintsTx = new List<int>();
            _pendingRequests = new List<MethodBase>();
            _alive = new Timer(TIMEOUT);
            _alive.Elapsed += IsAlive;
            _alive.Enabled = true;
            _master = master;
        }

        void IsAlive(object sender, ElapsedEventArgs e)
        {
            _master.ImAlive(_id);
        }

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

            _padintsTx.Add(uid);

            return p;
        }

        public bool Status()
        {
            return _status;
        }

        public bool Fail()
        {
            _fail = true;
            _status = false;

            return _fail;
        }

        public bool Freeze()
        {
            _freeze = true;
            _status = false;

            return _freeze;
        }

        public bool Recover()
        {
            bool ok = false;

            if (_fail || _freeze)
            {
                _fail = false;
                _freeze = false;
                _status = true;

                ok = DispatchPendindRequests();
            }

            return ok;
        }

        private bool DispatchPendindRequests()
        {
            try
            {
                foreach (MethodBase m in _pendingRequests)
                {
                    m.Invoke(this, m.GetParameters());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
            return true;
        }

		public bool DoCommit()
		{
            PADInt pad;
			foreach (int p in _padintsTx)
			{
                pad = _padints[p];
				pad.persistValue();
				_padintsTx.Clear();
				return true;
			}
			return false;
		}

		public bool DoAbort()
		{
			_padintsTx.Clear();
			return true;
		}

        public bool prepare()
        {
            throw new NotImplementedException();
        }

        public void LockPADInt(int uid, int timestamp)
		{
            if (_padintsTx.Contains(uid))
                throw new TxException("The PADInt" + uid + " is already locked!");
            else if (_padints[uid].Timestamp >= timestamp)
                throw new TxException("The client timestamp is lower than the object's timestamp!");
            else
			{
                _padintsTx.Add(uid);
            }
        }

        public void UnlockPADInt(int uid)
        {
            if (!_padintsTx.Contains(uid))
                throw new TxException("The PADInt" + uid + "is not locked");
            else
            {
                _padintsTx.Remove(uid);
            }
        }


        public string GetServerUrl() {
            return url;
        }


        public bool TxCommit(List<string> participants)
        {
            return true;/*ana lopes :3 */
        }

        public bool TxAbort()
        {
            throw new NotImplementedException();
        }
    }
}
