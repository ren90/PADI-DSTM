using DSTMLib;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace PADIClient
{
    public delegate void WriteDelegate(string s);



    static class ClientRunner
    {
 
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UserInterface());
        }
    }

	public class Client
	{
		
		private List<PADInt> padints = new List<PADInt>();
        public WriteDelegate _writeDelegate {get;set;}

        public Client()
        {

            DSTMLib.DSTMLib.init();
            padints = new List<PADInt>();

        }

        //public void connectToMaster(string url)
        //{
        //    ChannelServices.RegisterChannel(channel, false);

        //    master = (MasterInterface)Activator.GetObject(typeof(MasterInterface), url);
            
        //}

        //public void connectToServer(string url)
        //{
        //    channel = new TcpChannel();
        //    ChannelServices.RegisterChannel(channel, true);

        //    server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), url);
        //}

		public void CreatePADInt(int uid)
		{
			PADInt p = DSTMLib.DSTMLib.CreatePADInt(uid);
            _writeDelegate("created int with UID: " + uid);
		}

		public void AccessPADInt(int uid)
		{
            PADInt p = DSTMLib.DSTMLib.AccessPADInt(uid);

		}

		public void Read()
		{
			//TODO
		}

		public void Write(int value)
		{
			//TODO
		}
	}
}
