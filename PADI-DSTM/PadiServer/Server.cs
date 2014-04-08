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
            System.Console.WriteLine("Registered Channel @random" );

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

    class TransactionalServer : MarshalByRefObject, ServerInterface
	{
		// a collection of all the padints a server holds;
		// the correspondence is PADInt uid -> PADInt;
		// though the PADInt knows its own uid, this improves access speed
        private Dictionary<int, PADInt> _padints;
		// a list of all the padints involved in a given transaction
		private List<PADInt> _padintsTx;
		// list of client requests (access, create, etc) that haven't been executed;
		// this happens when a server is frozen
		private List<MethodBase> _pendingRequests;
		// true if the server is functioning correctly; false otherwise
		private bool _status { get; set; }
		// true if the server is simulating a fail situation; false otherwise
		private bool _fail { get; set; }
		// true if the server is simulating a freeze situation; false otherwise
		private bool _freeze { get; set; }

        public TransactionalServer()
		{
            _padints = new Dictionary<int, PADInt>();
			_padintsTx = new List<PADInt>();
			_pendingRequests = new List<MethodBase>();
		}

        public PADInt CreatePADInt(int uid, ServerInterface servers)
        {
			if (_padints.ContainsKey(uid))
				throw new TxException("SERVER: PADInt with uid " + uid + " already exists!");

			PADInt p = new PADInt(uid, servers);
			Console.WriteLine("SERVER: Created PADInt with uid: " + p.UID);
			
			_padints.Add(uid, p);
			Console.WriteLine("Added to dictionary");

			_padintsTx.Add(p);

			return p;
        }

        public PADInt AccessPADInt(int uid)
        {
            if (!_padints.ContainsKey(uid))
				throw new TxException("SERVER: PADInt with uid " + uid +" doesn't exist!");

			PADInt p = _padints[uid];
			Console.WriteLine("SERVER: Accessing PADInt with uid " + uid + "...");

			_padintsTx.Add(p);

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
				
				ok = dispatchPendindRequests();
			}

			return ok;
		}

		private bool dispatchPendindRequests()
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

		public bool TxBegin()
		{
			return true;
		}

		public bool TxCommit()
		{
			foreach (PADInt p in _padintsTx)
			{
				p.persistValue();
				_padintsTx.Clear();
				return true;
			}
			return false;
		}

		public bool TxAbort()
		{
			return true;
		}
    }
}
