using System;
using DSTMLib;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Net;

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
                if (!(ip.ToString().Contains("192.168")) && ip.AddressFamily.ToString() == "InterNetwork")
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
        private int _port { get; set; }
        private int _portseed { get; set; }
        private int _idseed { get; set; }
        // transactional server dictionary <Server id, server address> 
        private Dictionary<int, string> _transactionalServers;
        // PADInt references dictionary <PADInt id, server id list>
        private Dictionary<int, List<int>> _padintReferences;
        private int timestamps;


        public MasterServer()
        {
            _port = 8087;
            _portseed = 9000;
            _idseed = 0;
            _transactionalServers = new Dictionary<int, string>();
            _padintReferences = new Dictionary<int, List<int>>();
            timestamps = 0;

        }

        private string makeAddress(string host, int port)
        {
            return "tcp://" + host + ":" + port + "/Server";
        }

        //registers transactional servers and gives a port for them to bind on
        public KeyValuePair<int, int> registerTransactionalServer(string ip)
        {

            int id = _idseed;
            int port = _portseed;
            string address = makeAddress(ip, port);
            _idseed++;
            _portseed++;
            _transactionalServers.Add(id,address);

            Console.WriteLine("Registered new server!");
            Console.WriteLine("ID: " + id);
            Console.WriteLine("ADDRESS:" + address);

            return new KeyValuePair<int, int>(id, port);

        }

        public int hashServers(int seed){

            return (_padintReferences.Keys.Count + seed) % _transactionalServers.Keys.Count;

        }

        public Dictionary<int, string> generateServers (int uid){

            Dictionary<int, string> servers = new Dictionary<int, string>();
            
            int server = hashServers(0);
            servers.Add(server, _transactionalServers[server]);
            server = hashServers(1);
            servers.Add(server, _transactionalServers[server]);
            server = hashServers(2);
            servers.Add(server, _transactionalServers[server]);
            List<int> serverIds = servers.Keys.ToList<int>();
            _padintReferences.Add(uid, serverIds);
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

        public int getCoordinator(List<int> servers) {
            Random rnd = new Random();
            int server; 
            if (_transactionalServers.Count == 0)
                return -1;
            else {
                do
                {
                    server = rnd.Next(_transactionalServers.Count);
                }
                while (servers.Contains(server));
                return server;
            }
        }

        public int getTimestamp() {
            return timestamps++;
        }
    }

}
