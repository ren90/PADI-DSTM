using System.Collections.Generic;

namespace DSTMLib
{
    public interface MasterInterface
    {
        KeyValuePair<int, string> GenerateServers(int uid);
		KeyValuePair<int, int> RegisterTransactionalServer(string ip);
        KeyValuePair<int, int> GetTransactionData();
		string GetServers(int uid);
        string GetCoordinator();
        bool FinishTransaction(int uid);
        void ImAlive(int serverId);
    }


    //Transactionalm server interfaces

    public interface CoordinatorInterface
    {
        bool TxCommit(int tId, List<string> participants);
        bool TxAbort(List<string> participants, int tId);


    }

    public interface ParticipantInterface{

        void DoCommit(int tId, string coordinator);
        void DoAbort(int tId, string coordinator);
        void prepare(int tId, string coordinator);
        
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
