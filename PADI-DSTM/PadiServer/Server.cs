using DstmLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Diagnostics;

namespace PadiServer
{
    class ServerRunner
    {
        public static String serverBootstrap()
        {

            return "TS1";

        }

        static void Main(string[] args)
        {
            TcpChannel channel = new TcpChannel(8086);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServerInterface), serverBootstrap() , WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Registered Server");
            Debug.WriteLine("SERVER ON");
            System.Console.ReadLine();
        }
    }

    class TransactionalServer : MarshalByRefObject, ServerInterface
    {

        public TransactionalServer()
        {
            Console.WriteLine("Success!");

        }

        public int Read() { return 0; }

        public void Write(int value) { }
    }
}
