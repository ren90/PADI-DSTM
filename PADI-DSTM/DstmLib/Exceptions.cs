using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

namespace DSTMLIB
{
	[Serializable]
    public class TxException : RemotingException, ISerializable
    {
		public TxException() { }
        public TxException(string message) : base(message) { }
		public TxException(string message, Exception ex) : base(message, ex) { }
    }
}


    /*[Serializable]
    public class TxException : ApplicationException
    {
        public String msg;
    
        public TxException(string c)
        {
            msg = c;
        }
    
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("msg", msg);
        }
    }*/