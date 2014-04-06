using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Collections.Generic;

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
            ChannelServices.RegisterChannel(_channel, false);

            _servers = new Dictionary<int, ServerInterface>();
            _master = (MasterInterface)Activator.GetObject(typeof(MasterInterface), "tcp://localhost:8087/Server");

            return true;
        }

        public static bool TxBegin() { throw new NotImplementedException(); }

        public static bool TxCommit() { throw new NotImplementedException(); }

        public static bool TxAbort() { throw new NotImplementedException(); }

        public static bool Status() {
        }

        public static bool Fail(string URL) {
            try
            {
                ServerInterface server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), URL);
                return server.Fail();
            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace);
            }
        }

        public static bool Freeze(string URL) {
            try
            {
                ServerInterface server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), URL);
                return server.Freze();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public static bool Recover(string URL) {
            try
            {
                ServerInterface server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), URL);
                return server.Recover();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        // methods for creating and accessing PADInts

        public static PADInt CreatePADInt(int uid)
		{
        
            Console.WriteLine("DSTMLib-> calling master to create PADInt!");
            
            List<int> locations = _master.generateServers(uid);
            Console.Write("the chosen servers are: ");
            foreach (int port in locations)
                Console.Write(port.ToString() + ", ");
            Console.WriteLine();
            
            List<ServerInterface> tServers = new List<ServerInterface>();
            PADInt p;
            

            foreach (int port in locations){
                if (!_servers.ContainsKey(port)){
                    ServerInterface newServer = (ServerInterface)Activator.GetObject(typeof(ServerInterface), "tcp://localhost:"+ port + "/Server");
                    _servers.Add(port, newServer);
                    tServers.Add(newServer);
                }
                else
                    tServers.Add(_servers[port]);
            }

            p = _servers[locations[0]].CreatePADInt(uid, tServers);

            return p;

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
