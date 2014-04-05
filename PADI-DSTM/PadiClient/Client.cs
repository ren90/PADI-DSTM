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
		private List<PADInt> _padints;
        private WriteDelegate _logDelegate { get; set; }
        private WriteDelegate _listDelegate { get; set; }

        public Client(WriteDelegate logDelegate, WriteDelegate listDelegate)
        {
            DSTMLib.DSTMLib.init();
            _padints = new List<PADInt>();
            _logDelegate = logDelegate;
            _listDelegate = listDelegate;
        }

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
			if (!_padints.Contains(p))
			_padints.Add(p);
		}

		public int Read(int uid)
		{
			foreach (PADInt p in _padints)
			{
				if (p.UID == uid)
					return p.Read();
			}
			throw new TxException("An error occurred while reading from PADInt " + uid);
		}

		public void Write(int uid, int value)
		{
			foreach (PADInt p in _padints)
			{
				if (p.UID == uid)
					p.Write(value);
			}
			throw new TxException("An error occurred while writing to PADInt " + uid);
		}
	}
}
