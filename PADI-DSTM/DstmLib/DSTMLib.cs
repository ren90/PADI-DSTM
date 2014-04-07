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
        private static int timestamp;
        private static bool isInTransaction;

        // methods for manipulating PADI-DSTM

        public static bool Init()
		{
            _channel = new TcpChannel();
            ChannelServices.RegisterChannel(_channel, false);

            _servers = new Dictionary<int, ServerInterface>();
            _master = (MasterInterface)Activator.GetObject(typeof(MasterInterface), "tcp://localhost:8087/Server");
            isInTransaction = false;
            return true;
        }
        /// <summary>
        /// Starts a transaction, getting a new timestamp from the master.
        /// If it is already in a transaction, can't start a new transaction and returns false to the client.
        /// </summary>
        /// <returns></returns>
        public static bool TxBegin() {
            if (!isInTransaction){
                timestamp = _master.getTimestamp();
                isInTransaction = true;
                return true;
            }
            else return false;
        }

        public static bool TxCommit() {
            isInTransaction = false; //quando acabar a transaccao actualiza-se para falso, para a biblioteca poder receber novos TxBegin()
            return false;
        }

        public static bool TxAbort() { throw new NotImplementedException(); }

        public static bool Status()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calls the fail function in a given server, this makes the server stop
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static bool Fail(string URL) {
            try
            {
                ServerInterface server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), URL);
                return server.Fail();
            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }
        /// <summary>
        /// Calls the freeze function in the given function, in order to pause the server
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static bool Freeze(string URL) {
            try
            {
                ServerInterface server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), URL);
                return server.Freeze();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// Calls the recover function in a given server, this makes the server start to run again
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static bool Recover(string URL) {
            try
            {
                ServerInterface server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), URL);
                return server.Recover();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
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
        /// <summary>
        /// Function to get a remote reference to a PADInt with a given uid.
        /// If the object doesn't exist, then returns null, and warns the client.
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>The remote reference to the PADInt object</returns>
		public static PADInt AccessPADInt(int uid)
		{
            List<String> servers;
			Console.WriteLine("DSTMLib-> calling master to get the servers for the PADInt!");
			servers =  _master.GetServers(uid);

            if(servers == null){
               Console.WriteLine("ERROR: The PADInt does not exits");
               return null;
            }

            Random rnd = new Random();

            Console.WriteLine("DSTMLib-> connecting to the server to get the PADInt");
            ServerInterface chosen = (ServerInterface)Activator.GetObject(typeof(ServerInterface), servers[rnd.Next(4)]);
            
            return chosen.AccessPADInt(uid);
		}
    }
}
