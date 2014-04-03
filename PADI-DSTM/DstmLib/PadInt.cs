using System;
using System.Collections.Generic;

namespace DSTMLib {

    [Serializable]
    public class PADInt {
        private int _value;
        private int _uid;

        public int Value {
			get { return this._value; }
			private set { _value = value; }
		}
        
		public int UID {
			get { return this._uid; }
			private set { _uid = value; }
		}
		
		List<ServerInterface> _servers;

		public PADInt(int uid, List<ServerInterface> servers)
		{
			_uid = uid;
			_servers = servers;
            _value = 0;
		}

		public int Read() {
			return _value;
		}

		public void Write(int value) {
			_value = value;
		}
    }
}
