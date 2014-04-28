using System;
using System.Collections.Generic;

namespace DSTMLIB
{
    [Serializable]
    public class PADInt
	{
        //-----Values used by the PADInt object-----//
        private Dictionary<int, PADInt> _copies;
        // identifies unequivocally a PADInt
        private int _uid;
		// the value stored in the PADInt
        private int _value;
        List<string> _servers = new List<string>();


        //------------------------------------------//

        //-----Values used by the copy of the PADInt//
		// temporary value storage used in a transaction context;
		// only when the transaction commits, the temporary value becomes persistent
        private PADInt _originalValue;
		// used for concurrency control
        private int _timestamp;

        private int _transactionId;
        //------------------------------------------//
    
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
            _copies = new Dictionary<int, PADInt>();
		}

        public PADInt(PADInt p, int id)
        {
            _originalValue = p;
            _transactionId = id;
            _value = _originalValue.Value;
            _uid = _originalValue.UID;
            _originalValue.addCopy(this, id);
        }

        public PADInt originalPadint() {
            return _originalValue;
        }

		// "transform" the temporary value to persistent;
		// basically, the function writes to the _value field (which represents "persistency")
        public bool persistValue(int tId, int timestamp)
        {
            try
            {
                _value = getCopy(tId).Value;
                Timestamp = getCopy(tId).Timestamp;
                removeCopy(tId);
                return true;
            }
            catch (Exception e)
            {
                removeCopy(tId);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        public void rollback()
        {

            _value = _originalValue.Value;

        }

		public int Read()
		{
			Console.WriteLine("DSTMLib-> reading from PADInt " + this.UID + " with value " + this.Value);
            /*foreach (String server in _originalValue.getLocations())
            {
                ServerInterface serverLocation = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server);
                serverLocation.LockPADInt(_transactionId, UID, _timestamp);
            }*/
			return _value;
		}

		public void Write(int value)
		{
			Console.WriteLine("DSTMLib-> writing to PADInt " + this.UID + " the value " + value);
          /*  foreach (String server in _originalValue.getLocations())
            {
                ServerInterface serverLocation = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server);
                serverLocation.LockPADInt(_transactionId, UID, _timestamp);
            }*/
			_value = value;
		}

        public List<String> getLocations()
		{
			return _servers;
        }

        public void addCopy(PADInt copy, int tId) {
            _copies.Add(tId, copy);
        }

        public void removeCopy(int tId) {
            if (_copies.ContainsKey(tId)) {
                _copies.Remove(tId);            
            }
        }

        public PADInt getCopy(int tId) {
            if (_copies.ContainsKey(tId))
                return _copies[tId];
            else return null;
        }

    }
}
