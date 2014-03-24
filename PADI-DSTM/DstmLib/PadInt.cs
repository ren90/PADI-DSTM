using System.Collections.Generic;

namespace DSTMLib {
    public class PADInt {

		int _value { get; set; }
		List<ServerInterface> _servers;

		public PADInt(int initial_value, List<ServerInterface> servers)
		{
			_value = initial_value;
			_servers = servers;
		}

		public int Read() {
			return _value;
		}

		public void Write(int value) {
			_value = value;
		}
    }
}
