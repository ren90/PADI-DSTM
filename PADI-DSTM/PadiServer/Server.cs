using DSTMLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;


namespace PADIServer {
    class ServerRunner {

        static void Main(string[] args)
        {
            KeyValuePair<string, int> idAndPort;
            TcpChannel channel = new TcpChannel(8086);
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
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServerInterface), idAndPort.Key, WellKnownObjectMode.Singleton);
            System.Console.WriteLine("SERVER ON");
            System.Console.WriteLine("Name: " + idAndPort.Key + " Port: " + idAndPort.Value);
            System.Console.ReadLine();
        }
    }

    class TransactionalServer : MarshalByRefObject, ServerInterface {

        String mServer = System.IO.File.ReadAllText(@"C:\Users\Renato\workspace\VisualStudio\PADI-DSTM\mServerLocation.dat");
        
        public TransactionalServer()
		{

		}

		public int Read() { return 1; }

		public void Write(int value) { }



        
    }
}
