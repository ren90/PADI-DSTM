using DSTMLIB;
using System;
using System.Collections.Generic;
using System.Net;
using System.Timers;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PADIMaster
{
    class MasterRunner
    {
        static void Main(string[] args)
        {
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
        public static void OnTimeout(object sender, ElapsedEventArgs e, int serverId)
        {
            Console.WriteLine("O servidor " + serverId + " mooorrrrrreu!");
        }

        public void ImAlive(int tServerId)
        {
            _timers[tServerId].Interval = TIMEOUT;
            Console.WriteLine("server " + tServerId +" says: ALIVE");
        }

        public KeyValuePair<int, string> GenerateServers(int uid)
        {
            int server = HashServers(0);
            KeyValuePair<int, string> servers = new KeyValuePair<int, string>(server, _transactionalServers[server]);
            int serverId = servers.Key;
            _padintReferences.Add(uid, serverId);

            return servers;
        }

        public string GetServers(int uid)
        {
            Console.WriteLine("Received PADInt access request with the UID: " + uid);
            int serverId;
            List<String> addressList = new List<string>();

            if (_padintReferences.TryGetValue(uid, out serverId))
                return _transactionalServers[serverId];
            else
                return null;
        }

        public string GetCoordinator()
        {
            Random rnd = new Random();
            string url;
            int counter = _transactionalServers.Count;

            if (counter == 0)
                return "none";

			_transactionalServers.TryGetValue(rnd.Next(counter), out url);

            return url;
        }

        public KeyValuePair<int, int> GetTransactionData() {
            KeyValuePair<int, int> data = new KeyValuePair<int, int>(transactionsId++, timestamps++);
            transactionsInCourse.Add(data.Key);
            return data;
        }

        public bool FinishTransaction(int uid) {
            if (finishedTransactions.Contains(uid))
                return false;
            else finishedTransactions.Add(uid);
            return true;
        }
    }
}
