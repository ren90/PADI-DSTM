using System;
using System.Collections.Generic;
using System.Reflection;

namespace DSTMLIB
{
    public interface MasterInterface
    {
        List<string> GenerateServers(int uid);
		KeyValuePair<int, int> RegisterTransactionalServer(string ip);
		List<string> GetServers(int uid);
        string GetCoordinator();
        bool FinishTransaction(int uid);
        void ImAlive(int serverId, string address);
        int GetTimestamp();
        int GetTransactionID();
		List<int> GetServersToStore();
		Dictionary<int, string> GetAllServers();
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
        void DoAbort(int tId);
        void DoRollback(int tId);
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
        bool TxCommit(int tId, List<string> _servers, int timestamp);
        bool TxAbort(int tId, List<string> participants);
        string GetServerUrl();
		bool Fail_f();
		bool Freeze_f();
		void AddPendingRequest(MethodInfo methodInfo, List<Object> uid);

		string Dump();
	}
}
