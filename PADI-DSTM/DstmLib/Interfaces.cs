using System;
using System.Collections.Generic;
using System.Reflection;

namespace DSTMLIB
{
    public interface MasterInterface
    {
        Dictionary<int, string> GenerateServers(int uid);
		KeyValuePair<int, int> RegisterTransactionalServer(string ip);
		List<string> GetServers(int uid);
        string GetCoordinator();
        bool FinishTransaction(int uid);
        void ImAlive(int serverId, string address);
		void ReplicatePADInt(PADInt p, string url);
        int GetTimestamp();
        int GetTransactionID();
	}

    //Transactional server interfaces
    public interface CoordinatorInterface
    {
        bool TxCommit(int tId, List<string> _servers, int timestamp);
        bool TxAbort(int tId, List<string> participants);
        void ReceiveVote(bool reply);
    }

    public interface ParticipantInterface
	{
        bool DoCommit(int tId, string coordinator);
        void DoAbort(int tId, string coordinator);
        void Prepare(int tId, string coordinator, int timestamp);   
    }

	public interface ServerInterface
	{
        PADInt CreatePADInt(int uid, List<string> servers, int transactionId);
        PADInt AccessPADInt(int uid, int transactionId);
        bool Fail();
        bool Recover();
        bool Freeze();
		bool Status();
        void LockPADInt(int transactionId, int uid, int timestamp);
        void UnlockPADInt(int transactionId, int uid);
        bool TxCommit(int tId, List<string> _servers, int timestamp);
        bool TxAbort(int tId, List<string> participants);
        string GetServerUrl();
		bool Fail_f();
		bool Freeze_f();
		void AddPendingRequest(MethodInfo methodInfo, List<Object> uid);
		void ReplicatePADInt(PADInt p);
		List<PADInt> GetPADIntReferences();
	}
}
