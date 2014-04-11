using System;
using System.Collections.Generic;
using System.Reflection;

namespace DSTMLIB
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

    //Transactional server interfaces
    public interface CoordinatorInterface
    {
        bool TxCommit(int tId, List<string> participants, int timestamp);
        bool TxAbort(int tId, List<string> participants);
        void receiveVote(bool reply);
    }

    public interface ParticipantInterface
	{
        bool DoCommit(int tId, string coordinator);
        void DoAbort(int tId, string coordinator);
        void prepare(int tId, string coordinator, int timestamp);   
    }

	public interface ServerInterface
	{
        PADInt CreatePADInt(int uid, string servers);
        PADInt AccessPADInt(int uid);
        bool Fail();
        bool Recover();
        bool Freeze();
		bool Status();
        void LockPADInt(int transactionId, int uid, int timestamp);
        void UnlockPADInt(int transactionId, int uid);
        bool TxCommit(int tId, List<string> participants, int timestamp);
        bool TxAbort(int tId, List<string> participants);
        string GetServerUrl();
		bool Fail_f();
		bool Freeze_f();
		void AddPendingRequest(MethodInfo methodInfo, List<Object> uid);
	}
}
