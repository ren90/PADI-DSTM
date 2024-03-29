﻿using DSTMLIB;
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

        public Client(WriteDelegate logDelegate, WriteDelegate listDelegate)
        {
            DSTMLib.Init();
            _logDelegate = logDelegate;
            _listDelegate = listDelegate;
        }

		public void CreatePADInt(int uid)
		{
			try
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
			catch (TxException e)
			{
				_logDelegate(e.Message);
			}
		}

		public void AccessPADInt(int uid)
		{
			try
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
					_logDelegate("PADInt with uid " + uid + " could not be accessed!");
			}
			catch (TxException e)
			{
				_logDelegate(e.Message);
			}
		}

		public int Read(int uid)
		{
			try
			{
				int read_value;
				if (!_padints.ContainsKey(uid))
					throw new TxException("You haven't accessed PADInt " + uid);

				read_value = _padints[uid].Read();
				_logDelegate("PADInt with uid " + uid + " has value " + read_value);

				return read_value;
			}
			catch (TxException e)
			{
				_logDelegate(e.Message);
				return -1;
			}
		}

		public void Write(int uid, int value)
		{
			if (!_padints.ContainsKey(uid))
				throw new TxException("You haven't accessed PADInt " + uid + ", read/write operations not available.");
			try
			{
				_padints[uid].Write(value);
				_logDelegate("PADInt with uid " + uid + " written with value " + value);
			}
			catch (TxException e)
			{
				_logDelegate(e.Message);
			}
		}

        public void Status()
		{
			try
			{
				DSTMLib.Status();
				_logDelegate("Servers status:");
			}
			catch (TxException e)
			{
				_logDelegate(e.Message);
			}
		}

        public void Fail(string URL)
		{
			try
			{
				DSTMLib.Fail(URL);
				_logDelegate("Simulated Server fail @ " + URL);
			}
			catch (TxException e)
			{
				_logDelegate(e.Message);
			}
        }

        public void Freeze(string URL)
		{
			try
			{
				DSTMLib.Freeze(URL);
				_logDelegate("Simulated Server freeze @ " + URL);
			}
			catch (TxException e)
			{
				_logDelegate(e.Message);
			}
        }

        public void Recover(string URL)
		{
			try
			{
				DSTMLib.Recover(URL);
				_logDelegate("Server recovered @ " + URL);
			}
			catch (TxException e)
			{
				_logDelegate(e.Message);
			}
        }

        public bool TxBegin()
		{
			try
			{
				bool result = DSTMLib.TxBegin();
				_padints = new Dictionary<int, PADInt>();

				if (result)
					_logDelegate("Transaction started!");
				else
					_logDelegate("Cannot start transaction!");

				return result;
			}
			catch (TxException e)
			{
				_logDelegate(e.Message);
				return false;
			}
        }

        public void TxCommit()
		{
			try
			{
				_padints.Clear();
				bool result = DSTMLib.TxCommit();
				if (result)
					_logDelegate("Transaction successful");
				else
					_logDelegate("Transaction failed");
			}
			catch (TxException e)
			{
				_logDelegate(e.Message);
			}
		}

        public void TxAbort()
		{
			try
			{
				DSTMLib.TxAbort();
				_padints.Clear();
			}
			catch (TxException e)
			{
				_logDelegate(e.Message);
			}
		}
    }
}
