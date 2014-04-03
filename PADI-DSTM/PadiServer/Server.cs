using DSTMLib;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PADIServer {
    class ServerRunner {
        static void Main(string[] args)
        {
			KeyValuePair<int, int> idAndPort;
            TcpChannel channel = new TcpChannel();
            System.Console.WriteLine("Bootstrapping...");
            ChannelServices.RegisterChannel(channel, true);
            System.Console.WriteLine("Registered Channel @" + 8086);

            MasterInterface mServer = (MasterInterface)Activator.GetObject(typeof(MasterInterface), "tcp://localhost:8087/Server");
            idAndPort = mServer.registerTransactionalServer();

            TransactionalServer tServer = new TransactionalServer();
            System.Console.WriteLine("Registered at Master");
            ChannelServices.UnregisterChannel(channel);
            System.Console.WriteLine("Unbinding old port");
            channel = new TcpChannel(idAndPort.Value);
            ChannelServices.RegisterChannel(channel, true);
            System.Console.WriteLine("Registered Channel @" + idAndPort.Value);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(TransactionalServer), "Server", WellKnownObjectMode.Singleton);
            System.Console.WriteLine("SERVER ON");
            System.Console.WriteLine("Name: " + idAndPort.Key + " Port: " + idAndPort.Value);
            System.Console.ReadLine();
        }
    }

    class TransactionalServer : MarshalByRefObject, ServerInterface {
		String mServer = System.IO.File.ReadAllText(@"../../../../mServerLocation.dat");
        Dictionary<int, PADInt> _padints;

        public TransactionalServer()
		{
            _padints = new Dictionary<int, PADInt>();
		}

		public int Read() { return 1; }

		public void Write(int value) { }

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
    }
}
