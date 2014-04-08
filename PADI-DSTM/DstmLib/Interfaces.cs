using System.Collections.Generic;

namespace DSTMLib
{
    public interface MasterInterface
    {
        KeyValuePair<int, string> GenerateServers(int uid);
        string GetServers(int uid);
        KeyValuePair<int, int> RegisterTransactionalServer(string ip);
        int GetCoordinator(List<int> servers);
        int GetTimestamp();
    }

	public interface ServerInterface
	{
        PADInt CreatePADInt(int uid, ServerInterface servers);
        PADInt AccessPADInt(int uid);
        bool Fail();
        bool Recover();
        bool Freeze();
    }
}
