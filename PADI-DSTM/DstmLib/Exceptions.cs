using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace DSTMLIB
{
	[Serializable]
	public class TxException : RemotingException, ISerializable
	{
		public TxException() { }
		public TxException(string message) : base(message) { }
		public TxException(string message, Exception ex) : base(message, ex) { }

		[SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
