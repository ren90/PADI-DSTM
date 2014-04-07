using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

namespace DSTMLib
{
	[Serializable]
    public class TxException : RemotingException, ISerializable
    {
		public TxException() { }
        public TxException(string message) : base(message) { }
		public TxException(string message, Exception ex) : base(message, ex) { }
    }

}
