﻿using System;
using System.Collections.Generic;

namespace DSTMLIB
{
    [Serializable]
    public class PADInt : MarshalByRefObject
	{
        //-----Values used by the PADInt object-----//
        private Dictionary<int, PADInt> _temporaryValues;
        public Dictionary<int,PADInt> Temporary
		{
            get { return _temporaryValues; }
            private set { _temporaryValues = value; }
        }
        // identifies unequivocally a PADInt
        private int _uid;
        public int UID
		{
            get { return _uid; }
            private set { _uid = value; }
        }
		// the value stored in the PADInt
        private int _value;
        public int Value
		{
            get { return _value;}
            private set{_value = value;}
        }

        private List<String> _servers;
        public List<String> Servers
		{
            get { return _servers; }
            set { _servers = value; }
        }

        private int _oldValue;
        public int OldValue
		{
            get { return _oldValue; }
            set { _oldValue = value; }
        }
        //------------------------------------------//

        //-----Values used by the copy of the PADInt//
        //Location of the original Padints
        public List<PADInt> _originalValues;
        public List<PADInt> OriginalValues
        {
            get { return _originalValues; }
            set { _originalValues = value; }
        }
		// used for concurrency control
        private int _timestamp;
        public int Timestamp
        {
            get { return _timestamp; }
            private set { _timestamp = value; }
        }

        private int _transactionId;
        public int TransactioId
		{
            get { return _transactionId; }
            private set { _transactionId = value; }
        }
        //------------------------------------------//

		public PADInt(int uid, List<string> servers)
		{
			_uid = uid;
            _servers = servers;
            _value = 0;
           // _temporaryValues = new Dictionary<int, PADInt>();
		}

        public PADInt(List<PADInt> originals, int tId, int uid, int value)
        {
            _originalValues = originals;
            _value = value;
            _transactionId = tId;
            _uid = uid;
        }

		// "transform" the temporary value to persistent;
		// basically, the function writes to the _value field (which represents "persistency")
        public bool PersistValue(int tId, int timestamp)
        {
            try
            {
                _oldValue = _value;
                //_value = _temporaryValues[tId];
                Timestamp = timestamp;
                _temporaryValues.Remove(tId); 
                return true;
            }
            catch (Exception e)
            {   
                Console.WriteLine(e.StackTrace);
                return false;
            }
        }

        public void Rollback()
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
			Console.WriteLine("PadInt-> writing to PADInt " + this.UID + " the value " + value);
			_value = value;
		}

      /*  public void UpdateTemporary(int tId, int value) {
            if (!_temporaryValues.ContainsKey(tId))
                _temporaryValues.Add(tId, value);
            else
                _temporaryValues[tId] = value;
        }*/
    }
}
