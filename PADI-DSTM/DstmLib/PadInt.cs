
using System.Collections.Generic;

namespace DstmLib{
        
    public class PadInt {

            int _value { get; set; }
            List<ServerInterface> _servers;

            public PadInt(int initial_value, List<ServerInterface> servers)
            {
                _value = initial_value;
                _servers = servers;
            }

            //dummy, almost non-funcitonal methods
            public int Read() { return _value; }

            public void Write(int value) { _value = value;}
        }
}