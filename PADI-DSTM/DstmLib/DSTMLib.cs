using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections.Generic;

namespace DSTMLib
{
    public static class DSTMLib
    {
        private static TcpChannel _channel;
        private static MasterInterface _master;

        // methods for manipulating PADI-DSTM

        public static bool init()
		{
            _channel = new TcpChannel();
            ChannelServices.RegisterChannel(_channel, true);

            _master = (MasterInterface)Activator.GetObject(typeof(MasterInterface), "tcp://localhost:8087/Server");

            return true;
        }

        public static bool TxBegin() { throw new NotImplementedException(); }

        public static bool TxCommit() { throw new NotImplementedException(); }

        public static bool TxAbort() { throw new NotImplementedException(); }

        public static bool Status() { throw new NotImplementedException(); }

        public static bool Fail(string URL) { throw new NotImplementedException(); }

        public static bool Freeze(string URL) { throw new NotImplementedException(); }

        public static bool Recover(string URL) { throw new NotImplementedException(); }

        // methods for creating and accessing PADInts

        public static PADInt CreatePADInt(int uid)
		{
            Console.WriteLine("DSTMLib-> calling master to create PADInt!");
            return _master.CreatePADInt(uid);
		}

		public static PADInt AccessPADInt(int uid)
		{
            List<int> servers;
			Console.WriteLine("DSTMLib-> calling master to get the servers for the PADInt!");
			servers =  _master.GetServers(uid);

            if(servers == null){
               Console.WriteLine("ERROR: The PADInt does not exits");
               return null;
            }

            Console.WriteLine("DSTMLib-> connecting to the server to get the PADInt");
            ServerInterface chosen = (ServerInterface)Activator.GetObject(typeof(ServerInterface), "tcp://localhost:" + servers[0] + "/Server");
            
            return chosen.AccessPADInt(uid);
		}
    }
}
