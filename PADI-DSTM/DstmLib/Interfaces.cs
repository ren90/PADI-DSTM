using System.Collections.Generic;

namespace DSTMLib
{
    public interface MasterInterface
    {
        KeyValuePair<int, string> GenerateServers(int uid);
        string GetServers(int uid);
        KeyValuePair<int, int> RegisterTransactionalServer(string ip);
        string GetCoordinator();
        int GetTimestamp();
        void imAlive(int serverId);
    }

	public interface ServerInterface
	{
        PADInt CreatePADInt(int uid, ServerInterface servers);
        PADInt AccessPADInt(int uid);
        bool Fail();
        bool Recover();
        bool Freeze();
        void LockPadInt(int uid, int timestamp);
        void UnlockPadInt(int uid);
    }
}
