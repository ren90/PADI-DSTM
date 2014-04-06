using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace DSTMLib
{
    public static class DSTMLib
    {
        private static TcpChannel _channel;
        private static MasterInterface _master;
        private static Dictionary<int, ServerInterface> _servers;

        // methods for manipulating PADI-DSTM

        public static bool init()
		{
            _channel = new TcpChannel();
            ChannelServices.RegisterChannel(_channel, true);

            _servers = new Dictionary<int, ServerInterface>();
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
            List<int> locations = _master.generateServers(uid);
            List<ServerInterface> tServers = new List<ServerInterface>();
            

            foreach (int port in locations){
                if (!_servers.ContainsKey(port)){
                    ServerInterface newServer = (ServerInterface)Activator.GetObject(typeof(ServerInterface), "tcp://localhost:"+ port + "/Server";
                    _servers.Add(port, newServer);
                    tServers.Add(newServer);
                }
                else
                    tServers.Add(_servers[port]);
            }

            return _servers[locations[0]].CreatePADInt(uid, tServers);

        }

		public static PADInt AccessPADInt(int uid)
		{
			Console.WriteLine("DSTMLib-> calling master to access PADInt!");
			return _master.AccessPADInt(uid);
		}
    }
}
