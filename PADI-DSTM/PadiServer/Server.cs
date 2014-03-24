using DSTMLib;
using System;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PADIServer {
    class ServerRunner {
        public static String serverBootstrap() { return "TS1"; }

        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServerInterface), serverBootstrap(), WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Registered Server");
            System.Console.WriteLine("SERVER ON");
            System.Console.ReadLine();
        }
    }

    class TransactionalServer : MarshalByRefObject, ServerInterface {
		public TransactionalServer()
		{
			Console.WriteLine("Success!");
		}

		public int Read() { return 1; }

		public void Write(int value) { }
	}
}
