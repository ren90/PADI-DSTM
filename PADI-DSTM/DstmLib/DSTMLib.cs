using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace DSTMLIB
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
        private static bool set = false;

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
                if (!set)
                {
                    set = true;
                    RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
                }
                try
                {
                    foreach (PADInt p in _references)
                    {
                        foreach (String server in p.getLocations())
                        {
                            ServerInterface serverLocation = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server);
                            serverLocation.LockPADInt(transactionId, p.UID, timestamp);
                            serverList.Add(serverLocation.GetServerUrl());
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

        public static bool TxCommit()
		{
			CoordinatorInterface coordinator = (CoordinatorInterface)Activator.GetObject(typeof(CoordinatorInterface), transactionCoordinatorUrl);
			bool final_result = coordinator.TxCommit(transactionId, serverList, timestamp);

			#region Old Stuff
			//List<ServerInterface> _serversToCommit = new List<ServerInterface>();
			//foreach (string url in serverList)
			//{
			//	ServerInterface server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), url);
			//	_serversToCommit.Add(server);
			//}
			//
			//while (_serversToCommit.Capacity != 0)
			//{
			//	foreach (ServerInterface s in _serversToCommit)
			//	{
			//		bool result = s.TxCommit();
			//		if (result)
			//		{
			//			foreach (PADInt p in _references)
			//			{
			//				if (p.getLocations().Contains(s.GetServerURL()))
			//					s.UnlockPADInt(p.UID);
			//			}
			//			_serversToCommit.Remove(s);
			//		}
			//	}
			//}
			#endregion

			isInTransaction = false;
			return final_result;
        }

        public static bool TxAbort()
		{
			CoordinatorInterface coordinator = (CoordinatorInterface)Activator.GetObject(typeof(CoordinatorInterface), transactionCoordinatorUrl);
			coordinator.TxAbort(transactionId, serverList);

            isInTransaction = false;
            timestamp = -1;
            transactionId = -1;
            transactionCoordinatorUrl = "";
            serverList.Clear();
            _references.Clear();

			return true;
		}
		
        public static bool Status()
        {
			foreach (string server in serverList)
			{
				ServerInterface iserver = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server);
				if (iserver.Status())
					Console.WriteLine("Server " + server + " is up");
				else
					Console.WriteLine("Server " + server + " is down / not responding!");
				return true;
			}
			Console.WriteLine("All servers are down / not responding!");
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
            Console.WriteLine("DSTMLib-> calling master to create PADInt!");
            
            KeyValuePair<int, string> locations = _master.GenerateServers(uid);

            Console.Write("the chosen servers are: ");

            Console.WriteLine(locations.Value);

			ServerInterface tServer = (ServerInterface)Activator.GetObject(typeof(ServerInterface), locations.Value);
			if (tServer.Fail_f())
			{
				return null;
			}
			else if (tServer.Freeze_f())
			{
				Int32 parameter = uid;
				List<Object> parameters = new List<Object>();
				parameters.Add(parameter);

				tServer.AddPendingRequest(tServer.GetType().GetMethod("CreatePADInt"), parameters);
				return null;
			}

			PADInt reference = tServer.CreatePADInt(uid, locations.Value);
			_references.Add(reference);
            return reference;
        }

        // tem um insecto! faxabor de por isto a reotrnar os addresses faxabor
        /// <summary>
		/// Function to get a remote reference to a PADInt with a given uid.
		/// If the object doesn't exist, then returns null, and warns the client.
		/// </summary>
		/// <param name="uid"></param>
		/// <returns>The remote reference to the PADInt object</returns>
		public static PADInt AccessPADInt(int uid)
		{
            string servers;
			Console.WriteLine("DSTMLib-> calling master to get the servers for the PADInt!");
			servers =  _master.GetServers(uid);
            Console.Write("the chosen servers are: ");
            Console.WriteLine(servers);

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
			if (chosen.Fail_f())
			{
				return null;
			}
			else if (chosen.Freeze_f())
			{
				Int32 parameter = uid;
				List<Object> parameters = new List<Object>();
				parameters.Add(parameter);

				chosen.AddPendingRequest(chosen.GetType().GetMethod("AccessPADInt"), parameters);
				return null;
			}

            PADInt reference = chosen.AccessPADInt(uid);
            _references.Add(reference);

            return reference;
		}
    }
}
