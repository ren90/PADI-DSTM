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
            idAndPort = mServer.registerTransactionalServer(getIP());

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
		private String mServer = System.IO.File.ReadAllText(@"../../../mServerLocation.dat");
        private Dictionary<int, PADInt> _padints;
		private List<MethodBase> _pendingRequests = new List<MethodBase>();
		private bool _status { get; set; }
		private bool _fail { get; set; }
		private bool _freeze { get; set; }
		private TcpChannel channel;

        public TransactionalServer()
		{
            _padints = new Dictionary<int, PADInt>();
		}

        public PADInt CreatePADInt(int uid, List<ServerInterface> servers)
        {
            if (_padints.ContainsKey(uid))
                throw new TxException("PADInt with uid " + uid + " already exists!");

            PADInt p = new PADInt(uid, servers);
            Console.WriteLine("created PADInt with uid: " + p.UID);
            _padints.Add(uid, p);
            Console.WriteLine("added to dictionary");
            return p;
        }

        public PADInt AccessPADInt(int uid)
        {
            if (_padints.ContainsKey(uid))
            {
                Console.WriteLine("Contains!");
                return _padints[uid];
            }
            else
                throw new TxException("PADInt with identifier " + uid +" doesn't exist!");
        }

		public bool Status()
		{
			return _status;
		}

		public bool Fail()
		{
			_fail = true;
			_status = false;

			ChannelServices.UnregisterChannel(channel);

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
    }
}
