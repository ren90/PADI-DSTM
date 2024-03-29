﻿using System;
using System.Collections.Generic;
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
        private static Dictionary<int,PADInt> _references;
        private static List<string> _serverList;
        private static int _timestamp;
		// true if a transaction is occurring; false otherwise
        private static bool _isInTransaction;
        //The URL of the transactions coodinator
        private static string _transactionCoordinatorUrl;
        // methods for manipulating PADI-DSTM
        private static int _transactionId;

		public static bool Init()
		{
            _channel = new TcpChannel();
            ChannelServices.RegisterChannel(_channel, false);
            _master = (MasterInterface)Activator.GetObject(typeof(MasterInterface), "tcp://localhost:8087/Server");
            _isInTransaction = false;

			_references = new Dictionary<int, PADInt>();
			_serverList = new List<string>();

			return true;
        }

        /// <summary>
        /// Starts a transaction, getting a new timestamp from the master.
        /// If it is already in a transaction, a new transaction can't be started and returns false to the client.
        /// </summary>
        public static bool TxBegin()
		{
            if (!_isInTransaction)
            {
                _isInTransaction = true;
                _transactionId = _master.GetTransactionID();
                _timestamp = _master.GetTimestamp();

                return true;
            }
            else
            {
                Console.WriteLine("DSTMLib-> ERROR: There's already a transaction ocurring");
                return false;
            }
        }

        public static bool TxCommit()
		{
            _transactionCoordinatorUrl = _master.GetCoordinator();
            if (_transactionCoordinatorUrl == "")
                throw new TxException("404 Coordinator not found");

            foreach (PADInt localCopy in _references.Values)
			{
				foreach (PADInt original in localCopy.OriginalValues)
				{
					if (original != null)
						original.temporaryValue(localCopy.TransactioId, localCopy.Value);
				}
            }

			CoordinatorInterface coordinator = (CoordinatorInterface)Activator.GetObject(typeof(CoordinatorInterface), _transactionCoordinatorUrl);
            bool final_result = coordinator.TxCommit(_transactionId, _serverList, _timestamp);

            _master.FinishTransaction(_transactionId);

            clearVariables();
			
			return final_result;
        }

        public static bool TxAbort()
		{
            _transactionCoordinatorUrl = _master.GetCoordinator();
            if (_transactionCoordinatorUrl == "")
                throw new TxException("404 Coordinator not found");

			CoordinatorInterface coordinator = (CoordinatorInterface)Activator.GetObject(typeof(CoordinatorInterface), _transactionCoordinatorUrl);
			bool result = coordinator.TxAbort(_transactionId, _serverList);

            clearVariables();

			return result;
		}

        public static bool Status()
        {
			int nServers = _master.GetAllServers().Count;
			foreach (KeyValuePair<int, string> server in _master.GetAllServers())
			{
				ServerInterface iserver = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server.Value);
				if (iserver.Status())
					Console.WriteLine("Server " + server.Value + " is up");
				else
					Console.WriteLine("Server " + server.Value + " is down / not responding");
			}
			if (nServers > 0)
				return true;
			Console.WriteLine("All servers are down / not responding");
			return false;
        }

        /// <summary>
        /// Calls the fail function in a given server, this makes the server stop
        /// </summary>
        /// <param name="URL"></param>
        public static bool Fail(string URL)
		{
            ServerInterface server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), URL);
            return server.Fail();
        }

        /// <summary>
        /// Calls the freeze function in order to pause the server
        /// </summary>
        /// <param name="URL"></param>
        public static bool Freeze(string URL)
		{
            ServerInterface server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), URL);
            return server.Freeze();
        }

        /// <summary>
        /// Calls the recover function in a given server, this makes the server start to run again
        /// </summary>
        /// <param name="URL"></param>
        public static bool Recover(string URL)
		{
            ServerInterface server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), URL);
            return server.Recover();
        }

        // methods for creating and accessing PADInts

        public static PADInt CreatePADInt(int uid)
		{
			Console.WriteLine("DSTMLib-> calling master to create PADInt!");

			List<PADInt> objectReferences = new List<PADInt>();
			List<string> servers = new List<string>(); 
			List<string> locations = _master.GenerateServers(uid);
			
			if (locations == null)
			{
				Console.WriteLine("DSTMLib-> ERROR: There is already a PADInt with the uid: " + uid);
				return null;
			}
			else
			{
				foreach (string server in locations)
					servers.Add(server);
			}

			foreach (String server in servers)
			{
				ServerInterface tServer = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server);
				objectReferences.Add(tServer.CreatePADInt(uid, servers, _transactionId));

				if (!_serverList.Contains(server))
					_serverList.Add(server);
			}

            PADInt localCopy = new PADInt(objectReferences, _transactionId, objectReferences[0].UID, objectReferences[0].Value);
            _references.Add(localCopy.UID, localCopy);
            return localCopy;
        }

        /// <summary>
		/// Function to get a remote reference to a PADInt with a given uid.
		/// If the object doesn't exist, then returns null, and warns the client.
		/// </summary>
		/// <param name="uid"></param>
		/// <returns>The remote reference to the PADInt object</returns>
		public static PADInt AccessPADInt(int uid)
		{
            List<PADInt> objectReferences = new List<PADInt>();
            List<string> servers;

            if (_references.ContainsKey(uid))
            {
                Console.WriteLine("The reference for the PADInt " + uid + " was in cache");
                return _references[uid];
            }

            Console.WriteLine("DSTMLib-> calling master to get the servers for the PADInt!");
            servers = _master.GetServers(uid);

            if (servers == null)
            {
                Console.WriteLine("DSTMLib-> ERROR: The PadInt with uid" + uid + " does not exist");
                return null;
            }

            foreach (String server in servers) 
            {
                Console.WriteLine("DSTMLib-> connecting to the server " + server);
                ServerInterface chosen = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server);
                objectReferences.Add(chosen.AccessPADInt(uid, _transactionId));

                if (!_serverList.Contains(server))
                    _serverList.Add(server);
            }

            PADInt localCopy = new PADInt(objectReferences, _transactionId, objectReferences[0].UID, objectReferences[0].Value);
            _references.Add(localCopy.UID, localCopy);
            return localCopy;
		}

        private static void clearVariables()
        {
            _isInTransaction = false;
            _timestamp = -1;
            _transactionId = -1;
            _transactionCoordinatorUrl = "";
            _serverList.Clear();
            _references.Clear();
        }
    }
}
