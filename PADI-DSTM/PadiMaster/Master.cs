using DSTMLIB;
using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Net.Sockets;

namespace PADIMaster
{
    class MasterRunner
    {
        static void Main(string[] args)
        {
			RemotingConfiguration.Configure(@"../../App.config", true);

            int port = 8087;

            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, false);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MasterServer), "Server", WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Registered Master");
            string host;

            host = getIP();

            System.IO.File.WriteAllText(@"../../../mServerLocation.dat", "tcp://" + host + ":" + port + "/Server");
            
            System.Console.WriteLine("SERVER ON");
            System.Console.ReadLine();
        }

        private static string getIP()
        {
            IPHostEntry host;
            string localIP = "?";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    Console.WriteLine(localIP);
                }
            }
            return localIP;
        }
    }

    class MasterServer : MarshalByRefObject, MasterInterface
    {
		// the master's port id to comunicate
        private int _port { get; set; }
		// used for automatically generate port ids for the servers
        private int _portseed { get; set; }
		// used for automatically generate ids for the servers
        private int _idseed { get; set; }
        // transactional server dictionary <Server id, server address> 
        private Dictionary<int, string> _transactionalServers;
        // PADInt references dictionary <PADInt id, server id>
        private Dictionary<int, int> _padintReferences;
        // Server timer dictionary <server id, timer >
        private Dictionary<int, Timer> _timers;
		// timestamp generator for the PADInts
        private int timestamps;
        //Server timeout
        const double TIMEOUT = 10000;
		// a list containing the ids of all the transactions that finished
        private List<int> finishedTransactions;
		// a list containing the ids of all the transactions that haven't finished
        private List<int> transactionsInCourse;
		// id generator for the transactions
        private int transactionsId;
		// list of "dead" servers
		private List<string> _deadServers;

        public MasterServer()
        {
            _port = 8087;
            _portseed = 9000;
            _idseed = 0;
            _transactionalServers = new Dictionary<int, string>();
            _padintReferences = new Dictionary<int, int>();
            _timers = new Dictionary<int,Timer>();
            finishedTransactions = new List<int>();
            transactionsInCourse = new List<int>();
			_deadServers = new List<string>();
            timestamps = 0;
            transactionsId = 0;
        }

        private string makeAddress(string host, int port)
        {
            return "tcp://" + host + ":" + port + "/Server";
        }

        //registers transactional servers and gives a port for them to bind on
        public KeyValuePair<int, int> RegisterTransactionalServer(string ip)
        {
            int id = _idseed;
            int port = _portseed;
            string address = makeAddress(ip, port);
            Timer t = new Timer(TIMEOUT);
            _idseed++;
            _portseed++;
            _transactionalServers.Add(id,address);

            _timers.Add(id, t);
            t.Elapsed += (sender, e) => OnTimeout(sender, e, id);
            t.Enabled = true;

            Console.WriteLine("Registered new server!");
            Console.WriteLine("ID: " + id);
            Console.WriteLine("ADDRESS:" + address);

            return new KeyValuePair<int, int>(id, port);
        }

        public int HashServers(int seed)
        {
            return (_padintReferences.Keys.Count + seed) % _transactionalServers.Keys.Count;
        }

        //TODO ------------
        public void OnTimeout(object sender, ElapsedEventArgs e, int serverId)
        {
            Console.WriteLine("The server " + serverId + " is down!");
			_deadServers.Add(_transactionalServers[serverId]);
        }

        public void ImAlive(int tServerId)
        {
            _timers[tServerId].Interval = TIMEOUT;
            Console.WriteLine("server " + tServerId +" says: ALIVE");
        }

        public Dictionary<int, string> GenerateServers(int uid)
        {
            if (_padintReferences.ContainsKey(uid))
                return null;

            Dictionary<int, string> servers = new Dictionary<int, string>();
            int server = HashServers(0);

            //Adicionar o resto(Criar um foreach)
            KeyValuePair<int,string> serverPair = new KeyValuePair<int, string>(server, _transactionalServers[server]); 
            servers.Add(server, _transactionalServers[server]);
            int serverId = serverPair.Key;
            _padintReferences.Add(uid, serverId);

            return servers;
        }

        public List<string> GetServers(int uid)
        {
            Console.WriteLine("Received PADInt access request with the UID: " + uid);
            List<String> addressList = new List<string>();

            if (_padintReferences.ContainsKey(uid))
            {
				//MELHORAR ISTO adicionar todos
                addressList.Add(_transactionalServers[_padintReferences[uid]]);
                return addressList;
            }
            else
                return null;
        }

        public string GetCoordinator()
        {
            Random rnd = new Random();
            string url;
            int counter = _transactionalServers.Count;

            if (counter == 0)
                return "";

			_transactionalServers.TryGetValue(rnd.Next(counter), out url);

            return url;
        }

        public int getTransactionID(){
            int id = transactionsId++;
            transactionsInCourse.Add(id);
            return transactionsId;
        }

        public int getTimestamp(){
            return timestamps++;
        }

        public bool FinishTransaction(int tId)
		{
            if (finishedTransactions.Contains(tId))
                return false;
            else
				finishedTransactions.Add(tId);
            return true;
        }
    }
}
