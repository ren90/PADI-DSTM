using System.Collections.Generic;

namespace DSTMLib
{
    public interface MasterInterface
    {
        PADInt CreatePADInt(int uid);
        List<int> GetServers(int uid);
        KeyValuePair<int, int> registerTransactionalServer();
    }

	public interface ServerInterface
	{
		int Read();
		void Write(int value);
        PADInt CreatePADInt(int uid, List<ServerInterface> servers);
        PADInt AccessPADInt(int uid);
    }
}
