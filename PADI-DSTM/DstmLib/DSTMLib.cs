﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace DSTMLib
{
    public static class DSTMLib
    {
        private static TcpChannel _channel;
        private static MasterInterface _master;
        // transactional server cache <server id, server object>
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

        public static bool Status()
        {
            throw new NotImplementedException();
        }

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
            
            Dictionary<int, string> locations = _master.generateServers(uid);
            List<int> ids = locations.Keys.ToList<int>();
            Console.Write("the chosen servers are: ");
            foreach (int id in ids)
                Console.Write(locations[id].ToString() + ", ");
            Console.WriteLine();
            
            List<ServerInterface> tServers = new List<ServerInterface>();
            PADInt p;
            

            foreach (int id in ids){
                if (!_servers.ContainsKey(id)){
                    ServerInterface newServer = (ServerInterface)Activator.GetObject(typeof(ServerInterface), locations[id]);
                    _servers.Add(id, newServer);
                    tServers.Add(newServer);
                }
                else
                    tServers.Add(_servers[id]);
            }

            p = _servers[locations.First().Key].CreatePADInt(uid, tServers);

            return p;

        }

        //tem um insecto! faxabor de por isto a reotrnar os addresses faxabor
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
