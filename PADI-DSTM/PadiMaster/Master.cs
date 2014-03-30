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

            String id = "Server";
            int port = 8087;

            TcpChannel channel = new TcpChannel(port);
            ChannelServices.RegisterChannel(channel, true);

            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MasterServer), id, WellKnownObjectMode.Singleton);
            System.Console.WriteLine("Registered Server");
            System.Console.WriteLine("SERVER ON " + id.ToString());
            System.Console.ReadLine();

        }
    }

    class MasterServer : MarshalByRefObject, MasterInterface
	{
        private int _port { get; set; }
        private int _portseed { get; set; }
        private int _idseed { get; set; }
        private Dictionary<string, int> _transactionalServers;

        public MasterServer()
        {
            _port = 8087;
            _portseed = 9001;
            _idseed = 1;
            _transactionalServers = new Dictionary<string, int>();
        }

        //registers transactional servers and gives a port for them to bind on
        public KeyValuePair<string, int> registerTransactionalServer(){

            string id = "TS" + _idseed;
            int port = _portseed;
            _idseed++;
            _portseed++;
            _transactionalServers.Add(id, port);

            Console.WriteLine("Registered new server!");
            Console.WriteLine("ID: " + id);
            Console.WriteLine("PORT:" + port);

            return new KeyValuePair<string, int>(id, port);

            }

        public PADInt CreatePADInt(int uid)
        {
            throw new NotImplementedException();
        }

        public PADInt AccessPADInt(int uid)
        {

            throw new NotImplementedException();

        }
            

	    }
}
