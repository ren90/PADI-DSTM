using System.Collections.Generic;

namespace DSTMLib
{
    public interface MasterInterface
    {

        Dictionary<int, string> generateServers(int uid);
        List<int> GetServers(int uid);
        KeyValuePair<int, int> registerTransactionalServer(string ip);
    }

	public interface ServerInterface
	{

        PADInt CreatePADInt(int uid, List<ServerInterface> servers);
        PADInt AccessPADInt(int uid);
        bool Fail();
        bool Recover();
        bool Freeze();
    }
}
