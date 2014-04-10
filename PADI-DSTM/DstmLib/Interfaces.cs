﻿using System.Collections.Generic;

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
        bool TxCommit();
        bool TxAbort();


    }

    public interface ParticipantInterface{

        bool DoCommit();
		bool DoAbort();
        bool prepare();

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
