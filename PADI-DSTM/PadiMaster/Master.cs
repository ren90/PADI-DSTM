using System;
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
            _padintReferences = new Dictionary<int,List<int>>();

            
        }

        //registers transactional servers and gives a port for them to bind on
        public KeyValuePair<int, int> registerTransactionalServer(){

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


        public PADInt CreatePADInt(int uid)
        {   

            Console.WriteLine("Received PADInt create request!");
            Console.WriteLine("UID: " + uid);
            //funcao de hash, store tha bitches
            //very very dirty hack: amanda sempre para o mesmo server (the vanilla flavor) 
            int port = 9001;
            ServerInterface chosen = (ServerInterface) Activator.GetObject(typeof(ServerInterface), "tcp://localhost:" + port + "/Server");
            
            PADInt p = chosen.CreatePADInt(uid, new List<ServerInterface>());
            List<int> l = new List<int> (port);
            _padintReferences.Add(uid, l);

            return p;
        }

        public PADInt AccessPADInt(int uid)
        {

            Console.WriteLine("Received PADInt access request!");
            Console.WriteLine("UID: " + uid);
            //funcao de hash, store tha bitches
            //very very dirty hack: amanda sempre para o mesmo server (the vanilla flavor) 
            int port = 9001;
            ServerInterface chosen = (ServerInterface)Activator.GetObject(typeof(ServerInterface), "tcp://localhost:" + port + "/Server");

            PADInt p = chosen.AccessPADInt(uid);

            return p;

        }
            

	    }
}
