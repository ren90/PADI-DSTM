using System.Collections.Generic;
namespace DSTMLib
{
    public interface MasterInterface
    {
        PADInt CreatePADInt(int uid);
        PADInt AccessPADInt(int uid);
        
        KeyValuePair<int, int> registerTransactionalServer();

    }

	public interface ServerInterface
	{
		int Read();
		void Write(int value);
	}
}
