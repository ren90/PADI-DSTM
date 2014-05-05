using System;
using System.Collections.Generic;

namespace DSTMLIB
{
    [Serializable]
    public class PADInt
	{
        //-----Values used by the PADInt object-----//
        private Dictionary<int, int> _temporaryValues;
        // identifies unequivocally a PADInt
        private int _uid;
		// the value stored in the PADInt
        private int _value;
        List<string> _servers = new List<string>();
        private int _oldValue;

        //------------------------------------------//

        //-----Values used by the copy of the PADInt//
		// temporary value storage used in a transaction context;
		// only when the transaction commits, the temporary value becomes persistent
        private List<PADInt> _originalValues;
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
		
		public PADInt(int uid, List<string> servers)
		{
			_uid = uid;
            _servers = servers;
            _value = 0;
            _temporaryValues = new Dictionary<int, int>();
		}

        public PADInt(List<PADInt> originals, int id)
        {
            _originalValues = new List<PADInt>();
            _value = originals[0].Value;
            _transactionId = id;
            _uid = originals[0].UID;
            
            foreach (PADInt p in originals) {
                _originalValues.Add(p);
                p.UpdateTemporay(_transactionId, _value);
            }  
        }

        public List<PADInt> originalPadint() {
            return _originalValues;
        }

		// "transform" the temporary value to persistent;
		// basically, the function writes to the _value field (which represents "persistency")
        public bool persistValue(int tId, int timestamp)
        {
            try
            {
                _oldValue = _value;
                _value = _temporaryValues[tId];
                Timestamp = timestamp;
                _temporaryValues.Remove(tId);
                return true;
            }
            catch (Exception e)
            {
                _temporaryValues.Remove(tId);
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        public void rollback()
        {
            _value = _oldValue;
        }

		public int Read()
		{
			Console.WriteLine("DSTMLib-> reading from PADInt " + this.UID + " with value " + this.Value);
			return _value;
		}

		public void Write(int value)
		{
			Console.WriteLine("DSTMLib-> writing to PADInt " + this.UID + " the value " + value);
			_value = value;
            foreach (PADInt o in _originalValues)
                o.UpdateTemporay(_transactionId, value);
		}

        public List<String> getLocations()
		{
			return _servers;
        }

        public void UpdateTemporay(int tId, int value) {
            if (!_temporaryValues.ContainsKey(tId))
                _temporaryValues.Add(tId, value);
            else _temporaryValues[tId] = value;
        }
    }
}
