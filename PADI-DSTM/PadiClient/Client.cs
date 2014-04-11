using DSTMLIB;
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
            DSTMLIB.DSTMLib.Init();
            _padints = new Dictionary<int, PADInt>();
            _logDelegate = logDelegate;
            _listDelegate = listDelegate;
        }

		public void CreatePADInt(int uid)
		{
			PADInt p = DSTMLIB.DSTMLib.CreatePADInt(uid);
            _padints.Add(uid, p);
            _logDelegate("created int with UID: " + uid);
            _listDelegate("UID:" + uid);
		}

		public void AccessPADInt(int uid)
		{
            PADInt p = DSTMLIB.DSTMLib.AccessPADInt(uid);
            
            if (p == null)
			{
                _logDelegate("The PadInt with the uid " + uid + " does not exist");
                return;
            }

			if (!_padints.ContainsKey(uid))
				_padints.Add(uid, p);

            _logDelegate("accessed int with UID: " + uid);
            _listDelegate("UID:" + uid);
		}

		public int Read(int uid)
		{
			int read_value;
			if (!_padints.ContainsKey(uid))
				throw new TxException("An error occurred while reading from PADInt " + uid);

			read_value = _padints[uid].Read();
			_logDelegate("PADInt " + uid + " has value " + read_value);
			return read_value;
		}

		public void Write(int uid, int value)
		{
			if (!_padints.ContainsKey(uid))
				throw new TxException("An error occurred while writing to PADInt " + uid);

			_padints[uid].Write(value);
			_logDelegate("PADInt " + uid + " written with value " + value);				
		}

        public void Status()
		{
			DSTMLIB.DSTMLib.Status();
		}

        public void Fail(string URL)
		{    
            DSTMLIB.DSTMLib.Fail(URL);
            _logDelegate("Simulated Server fail @" + URL);
        }

        public void Freeze(string URL)
		{
            DSTMLIB.DSTMLib.Freeze(URL);
            _logDelegate("Simulated Server freeze @" + URL);
        }

        public void Recover(string URL)
		{
            DSTMLIB.DSTMLib.Recover(URL);
            _logDelegate("Server recovered @" + URL);
        }

        public void TxBegin()
		{
            bool result = DSTMLIB.DSTMLib.TxBegin();

            if (result)
				_logDelegate("Transaction started!");
            else
				_logDelegate("Can not start transaction!");
        }

        public void TxCommit()
		{
			DSTMLIB.DSTMLib.TxCommit();
		}

        public void TxAbort()
		{
			DSTMLIB.DSTMLib.TxAbort();
		}
    }
}
