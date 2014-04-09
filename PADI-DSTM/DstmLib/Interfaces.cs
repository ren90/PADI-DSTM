using System.Collections.Generic;

namespace DSTMLib
{
    public interface MasterInterface
    {
        KeyValuePair<int, string> GenerateServers(int uid);
		KeyValuePair<int, int> RegisterTransactionalServer(string ip);
		string GetServers(int uid);
        string GetCoordinator();
        int GetTimestamp();
        void ImAlive(int serverId);
    }

	public interface ServerInterface
	{
        PADInt CreatePADInt(int uid, ServerInterface servers);
        PADInt AccessPADInt(int uid);
        bool Fail();
        bool Recover();
        bool Freeze();
		bool Status();
        void LockPADInt(int uid, int timestamp);
        void UnlockPADInt(int uid);
		bool TxBegin();
		bool TxCommit();
		bool TxAbort();
    }
}
