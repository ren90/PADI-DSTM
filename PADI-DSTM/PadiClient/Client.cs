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
		
		private List<PADInt> _padints = new List<PADInt>();
        private WriteDelegate _logDelegate {get;set;}
        private WriteDelegate _listDelegate { get; set; }

        public Client(WriteDelegate logDelegate, WriteDelegate listDelegate)
        {

            DSTMLib.DSTMLib.init();
            _padints = new List<PADInt>();
            _logDelegate = logDelegate;
            _listDelegate = listDelegate;
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
            _padints.Add(p);
            _logDelegate("created int with UID: " + uid);
            _listDelegate("UID:" + uid);
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
