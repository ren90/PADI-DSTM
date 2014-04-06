﻿using System;
using DSTMLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;

namespace PADIMaster
{
    class MasterRunner
    {
        static void Main(string[] args)
        {

            int port = 8087;

            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MasterServer), "Server", WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Registered Server");
            System.Console.WriteLine("SERVER ON");
            System.Console.ReadLine();

        }
    }

    class MasterServer : MarshalByRefObject, MasterInterface
    {
        private int _port { get; set; }
        private int _portseed { get; set; }
        private int _idseed { get; set; }
        private Dictionary<int, int> _transactionalServers;
        private Dictionary<int, List<int>> _padintReferences;


        public MasterServer()
        {
            _port = 8087;
            _portseed = 9001;
            _idseed = 1;
            _transactionalServers = new Dictionary<int, int>();
            _padintReferences = new Dictionary<int, List<int>>();
            List<int> o = new List<int>();
            o.Add(1);
            o.Add(2);
            o.Add(3);
            _padintReferences.Add(1, o);


        }

        //registers transactional servers and gives a port for them to bind on
        public KeyValuePair<int, int> registerTransactionalServer()
        {

            int id = _idseed;
            int port = _portseed;
            _idseed++;
            _portseed++;
            _transactionalServers.Add(id, port);

            Console.WriteLine("Registered new server!");
            Console.WriteLine("ID: " + id);
            Console.WriteLine("PORT:" + port);

            return new KeyValuePair<int, int>(id, port);

        }

        public List<int> generateServers (int uid){

            List<int> servers = new List<int>();
            servers.Add(_transactionalServers[0]);
            servers.Add(_transactionalServers[1]);
            servers.Add(_transactionalServers[2]);
            _padintReferences.Add(uid, servers);
            return servers;
        }


        public List<int> GetServers(int uid)
        {
            Console.WriteLine("Received PADInt access request with the UID: " + uid);
            List<int> serversList;
            if (_padintReferences.TryGetValue(uid, out serversList))
                return serversList;
            else
                return null;

        }
    }

}
