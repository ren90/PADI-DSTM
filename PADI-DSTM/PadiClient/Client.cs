using DSTMLib;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;

namespace PADIClient
{
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
		private TcpChannel channel;
		private MasterInterface master;
		private ServerInterface server;
		private List<PADInt> padints = new List<PADInt>();

		public void connectToMaster(string url)
		{
			channel = new TcpChannel();
			ChannelServices.RegisterChannel(channel, false);

			master = (MasterInterface)Activator.GetObject(typeof(MasterInterface), url);
            
        }

		public void connectToServer(string url)
		{
			channel = new TcpChannel();
			ChannelServices.RegisterChannel(channel, true);

			server = (ServerInterface)Activator.GetObject(typeof(ServerInterface), url);
		}

		public void CreatePADInt(int uid)
		{
			PADInt p = master.CreatePADInt(uid);
			if (p != null)
				padints.Add(p);
		}

		public void AccessPADInt(int uid)
		{
			PADInt p = master.AccessPADInt(uid);
			if (p != null)
				padints.Add(p);
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
