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
		private Dictionary<int, PADInt> _padints;
        private WriteDelegate _logDelegate { get; set; }
        private WriteDelegate _listDelegate { get; set; }

        public Client(WriteDelegate logDelegate, WriteDelegate listDelegate)
        {
            DSTMLib.DSTMLib.init();
            _padints = new Dictionary<int, PADInt>();
            _logDelegate = logDelegate;
            _listDelegate = listDelegate;
        }

		public void CreatePADInt(int uid)
		{
			PADInt p = DSTMLib.DSTMLib.CreatePADInt(uid);
            _padints.Add(uid, p);
            _logDelegate("created int with UID: " + uid);
            _listDelegate("UID:" + uid);
		}

		public void AccessPADInt(int uid)
		{
            PADInt p = DSTMLib.DSTMLib.AccessPADInt(uid);
			if (!_padints.ContainsKey(uid))
			_padints.Add(uid, p);
            _logDelegate("accessed int with UID: " + uid);
            _listDelegate("UID:" + uid);
		}

		public int Read(int uid)
		{
            if(_padints.ContainsKey(uid))
					return _padints[uid].Read();
			throw new TxException("An error occurred while reading from PADInt " + uid);
		}

		public void Write(int uid, int value)
		{
               if (_padints.ContainsKey(uid))
                    _padints[uid].Write(value);
               else
                   throw new TxException("An error occurred while writing to PADInt " + uid);
			
		}
	}
}
