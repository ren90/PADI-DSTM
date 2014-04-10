using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace DSTMLib
{
    public static class DSTMLib
    {
		// the channel used to comunicate with the client
        private static TcpChannel _channel;
		// the master remote interface
        private static MasterInterface _master;
      
        private static List<PADInt> _references;
        private static List<string> serverList;
        private static int timestamp;
		// true if a transaction is occurring; false otherwise
        private static bool isInTransaction;
        //The URL of the transactions coodinator
        private static string transactionCoordinatorUrl;
        // methods for manipulating PADI-DSTM
        private static int transactionId;

        public static bool Init()
		{
            _channel = new TcpChannel();
            ChannelServices.RegisterChannel(_channel, false);
            _master = (MasterInterface)Activator.GetObject(typeof(MasterInterface), "tcp://localhost:8087/Server");
            isInTransaction = false;
            _references = new List<PADInt>();
            serverList = new List<string>();

			return true;
        }

        /// <summary>
        /// Starts a transaction, getting a new timestamp from the master.
        /// If it is already in a transaction, a new transaction can't be started and returns false to the client.
        /// </summary>
        public static bool TxBegin()
		{
            if (!isInTransaction)
            {
                if (_references.Count == 0)
                {
                    Console.WriteLine("DSTMLib->ERROR: there are no PADInt references to make a transaction!");
                    return false;
                }
                KeyValuePair<int, int> data = _master.GetTransactionData();
                transactionId = data.Key;
                timestamp = data.Value;
                isInTransaction = true;
                transactionCoordinatorUrl = _master.GetCoordinator();

                try
                {
                    foreach (PADInt p in _references)
                    {
                        foreach (String server in p.getLocations())
                        {
                            ServerInterface serverLocation = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server);
                            serverLocation.LockPADInt(p.UID, timestamp);
                            serverList.Add(serverLocation.GetServerURL());
                        }
                    }
                }
                catch (TxException e)
                {
                    Console.WriteLine("DSTMLib->ERROR: " + e.Message);
                    return false;
                }

                return true;
            }
            else
            {
                Console.WriteLine("DSTMLib-> ERROR: There is already a started transaction");
                return false;
            }
        }

		// IMPEDE QUE OCORRAM MAIS TRANSACÇOES!
		// TEM QUE SER REFEITO
        public static bool TxCommit()
		{
			List<ServerInterface> _serversToCommit = new List<ServerInterface>();
			_serversToCommit = _servers.Values.ToList<ServerInterface>();
			
			bool final_result = true;

			while (_serversToCommit.Capacity != 0)
			{
				foreach (ServerInterface s in _serversToCommit)
				{
					bool result = s.TxCommit();
					_serversToCommit.Remove(s);
				}
			}

            isInTransaction = false; //quando acabar a transaccao actualiza-se para falso, para a biblioteca poder receber novos TxBegin()
			return final_result;
        }

        public static bool TxAbort()
		{
			bool final_result = true;
            foreach (string url in serverList) {

            try
                {
                    ServerInterface server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), url);
                    foreach (PADInt p in _references)
                        server.UnlockPADInt(p.UID);
                }
                catch (TxException e) {}
            }

            isInTransaction = false;
            timestamp = -1;
            transactionId = -1;
            transactionCoordinatorUrl = "";
            serverList.Clear();
            _references.Clear();

			return final_result;
		}
		
        public static bool Status()
        {
			/*foreach (KeyValuePair<int, ServerInterface> entry in _servers)
			{
				if (entry.Value.Status())
					Console.WriteLine("Server " + entry.Key + " is up");
				else
					Console.WriteLine("Server " + entry.Key + " is down / not responding!");
				return true;
			}
			Console.WriteLine("All servers are down / not responding!");*/
			return false;
        }

        /// <summary>
        /// Calls the fail function in a given server, this makes the server stop
        /// </summary>
        /// <param name="URL"></param>
        public static bool Fail(string URL)
		{
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
        /// Calls the freeze function in order to pause the server
        /// </summary>
        /// <param name="URL"></param>
        public static bool Freeze(string URL)
		{
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
        public static bool Recover(string URL)
		{
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
			string server_location = "";

            Console.WriteLine("DSTMLib-> calling master to create PADInt!");
            
            KeyValuePair<int, string> locations = _master.GenerateServers(uid);
            Console.Write("the chosen servers are: ");
            Console.Write(locations.Value);
            
            ServerInterface tServer = (ServerInterface)Activator.GetObject(typeof(ServerInterface), locations.Value);

            PADInt reference = tServer.CreatePADInt(uid, server_location);
            _references.Add(reference);
            return reference;
        }

        //tem um insecto! faxabor de por isto a reotrnar os addresses faxabor
        // <summary>
        // Function to get a remote reference to a PADInt with a given uid.
        // If the object doesn't exist, then returns null, and warns the client.
        // </summary>
        // <param name="uid"></param>
        // <returns>The remote reference to the PADInt object</returns>
		public static PADInt AccessPADInt(int uid)
		{
            string servers;
			Console.WriteLine("DSTMLib-> calling master to get the servers for the PADInt!");
			servers =  _master.GetServers(uid);

            if (isInTransaction)
            {
                Console.WriteLine("DSTMLib-> ERROR: There is already a transaction started");
                throw new TxException("There is already a transaction started");
            }

            if (servers == null)
			{
               Console.WriteLine("ERROR: The PADInt does not exist!");
               return null;
            }

            Console.WriteLine("The PADInts are at these servers: ");
            Console.WriteLine(servers.ToString());
            Console.WriteLine("DSTMLib-> connecting to the server to get the PADInt");
            
            ServerInterface chosen = (ServerInterface)Activator.GetObject(typeof(ServerInterface), servers);
            
            PADInt reference = chosen.AccessPADInt(uid);
            _references.Add(reference);

            return reference;
		}
    }
}
