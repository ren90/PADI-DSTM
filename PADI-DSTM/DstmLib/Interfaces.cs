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
        int GetTransactionId();
        bool FinishTransaction(int uid);
        bool StartTransaction(int uid);
        void ImAlive(int serverId);
    }


    //Transactionalm server interfaces

    public interface CoordinatorInterface
    {
        bool TxCommit(int tId, List<string> participants);
        bool TxAbort(int tId);


    }

    public interface ParticipantInterface{

        void DoCommit(int tId);
		void DoAbort(int tId);
        void prepare(int tId);
        
    }

	public interface ServerInterface
	{
        PADInt CreatePADInt(int uid, string servers);
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
        string GetServerURL();
    }
}
