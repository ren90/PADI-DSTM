using System;
using System.Collections.Generic;

namespace DSTMLIB
{
    [Serializable]
    public class PADInt
	{
		// the value stored in the PADInt
        private int _value;
		// temporary value storage used in a transaction context;
		// only when the transaction commits, the temporary value becomes persistent
        private int _temporaryValue;
		// identifies unequivocally a PADInt
        private int _uid;
		// used for concurrency control
        private int _timestamp;
        
        List<string> _servers = new List<string>();

        public int Timestamp
        {
            get { return this._value; }
            private set {_timestamp = value;}
        }

        public int Value
		{
			get { return this._value; }
			private set { _value = value; }
		}
        
		public int UID
		{
			get { return this._uid; }
			private set { _uid = value; }
		}
		
		public PADInt(int uid, string server)
		{
			_uid = uid;
			_servers.Add(server);
            _value = 0;
		}

		// "transform" the temporary value to persistent;
		// basically, the function writes to the _value field (which represents "persistency")
        public bool persistValue(int timestamp)
		{
            try
            {
                _value = _temporaryValue;
                _timestamp = timestamp;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        public void rollback()
        {

            _temporaryValue = _value;

        }

		public int Read()
		{
			Console.WriteLine("DSTMLib-> reading from PADInt " + this.UID + " with value " + this.Value);
			return _temporaryValue;
		}

		public void Write(int value)
		{
			Console.WriteLine("DSTMLib-> writing to PADInt " + this.UID + " the value " + value);
			_temporaryValue = value;
		}

        public List<String> getLocations()
		{
			return _servers;
        }
    }
}
