using DSTMLIB;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
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
			RemotingConfiguration.Configure(@"../../App.config", true);

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
		private int _serversFreezed = 0;

        public Client(WriteDelegate logDelegate, WriteDelegate listDelegate)
        {
            DSTMLib.Init();
            _padints = new Dictionary<int, PADInt>();
            _logDelegate = logDelegate;
            _listDelegate = listDelegate;
        }

		public void CreatePADInt(int uid)
		{
            PADInt p = DSTMLib.CreatePADInt(uid);

            if (p != null)
			{
                _padints.Add(uid, p);
                _logDelegate("PADInt with uid " + uid + "created!");
                _listDelegate("UID:" + uid);
            }
            else
                _logDelegate("ERROR: PADInt with uid " + uid + " already exists!");
		}

		public void AccessPADInt(int uid)
		{
            PADInt p = DSTMLib.AccessPADInt(uid);

            if (p != null)
			{
                if (!_padints.ContainsKey(uid))
                _padints.Add(uid, p);

                _logDelegate("PADInt with uid " + uid + " accessed!");
                _listDelegate("UID:" + uid);
            }
            else 
                _logDelegate("PADInt with uid "+ uid + " could not be accessed!");
		}

		public int Read(int uid)
		{
			int read_value;
			if (!_padints.ContainsKey(uid))
				throw new TxException("An error occurred while reading from PADInt " + uid);

			read_value = _padints[uid].Read();
			_logDelegate("PADInt with uid " + uid + " has value " + read_value);
			
			return read_value;
		}

		public void Write(int uid, int value)
		{
			if (!_padints.ContainsKey(uid))
				throw new TxException("An error occurred while writing to PADInt " + uid);

			_padints[uid].Write(value);
			_logDelegate("PADInt with uid " + uid + " written with value " + value);				
		}

        public void Status()
		{
			DSTMLib.Status();
		}

        public void Fail(string URL)
		{
			try
			{
				DSTMLib.Fail(URL);
				_logDelegate("Simulated Server fail @ " + URL);
			}
			catch (RemotingException e)
			{
				_logDelegate(e.Message);
			}
        }

        public void Freeze(string URL)
		{
			try
			{
				DSTMLib.Freeze(URL);
				_serversFreezed++;
				_logDelegate("Simulated Server freeze @ " + URL);
			}
			catch (RemotingException e)
			{
				_logDelegate(e.Message);
			}
        }

        public void Recover(string URL)
		{
			try
			{
				DSTMLib.Recover(URL);
				if (_serversFreezed < 0)
					_serversFreezed = 0;
				_logDelegate("Server recovered @ " + URL);
			}
			catch (RemotingException e)
			{
				_logDelegate(e.Message);
			}
        }

        public bool TxBegin()
		{
            bool result = DSTMLib.TxBegin();

            if (result)
				_logDelegate("Transaction started!");
            else
				_logDelegate("Cannot start transaction!");

            return result;
        }

        public void TxCommit()
		{
			if (_serversFreezed > 0)
			{
				_logDelegate("All servers must be unfreezed before commiting!");
			}
			else
			{
				bool result = DSTMLib.TxCommit();
				if (result)
					_logDelegate("Transaction successful");
				else
					_logDelegate("Transaction failed");
			}
		}

        public void TxAbort()
		{
			DSTMLib.TxAbort();
		}
    }
}
