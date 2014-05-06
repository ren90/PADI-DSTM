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
        public List<String> _originalValuesServers;
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

        public List<String> Servers {
            get { return _servers; }
        }

        public List<String> OriginalServers
        {
            get { return _originalValuesServers; }
        }
		
		public PADInt(int uid, List<string> servers)
		{
			_uid = uid;
            _servers = servers;
            _value = 0;
            _temporaryValues = new Dictionary<int, int>();
		}

        public PADInt(List<String> originals, int tId, int uid, int value)
        {
            _originalValuesServers = originals;
            _value = value;
            _transactionId = tId;
            _uid = uid;
        }

        public List<String> originalPadintServers() {
            return _originalValuesServers;
        }

		// "transform" the temporary value to persistent;
		// basically, the function writes to the _value field (which represents "persistency")
        public bool persistValue(int tId, int timestamp)
        {
            Console.WriteLine("ESTOU NO PERSIST VALUE " + tId);

            if (_temporaryValues.Keys.Count == 0)
            {
                Console.WriteLine("Estou vazio");
            }
            else Console.WriteLine("Não estou vazio");

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

            Console.WriteLine("HELLO: " + _originalValuesServers[0]);

            foreach (String server in OriginalServers)
            {
                    Console.WriteLine("Estou a fazer update ao padint original no servidor "+ server);
                    ServerInterface iserver = (ServerInterface)Activator.GetObject(typeof(ServerInterface), server);
                    iserver.updatePadintTemporaryValue(UID, _transactionId, value);
            }
		}

        public void UpdateTemporay(int tId, int value) {
            if (!_temporaryValues.ContainsKey(tId))
            {
                Console.WriteLine("Estou a fazer update com o tid: " + tId + "e valor: " + value);
                _temporaryValues.Add(tId, value);
                _temporaryValues[tId] = value;
            }
            else
            {
                Console.WriteLine("Estou a fazer update com o tid: " + tId + "e valor: "+ value);
                _temporaryValues[tId] = value;
            }
        }
    }
}
