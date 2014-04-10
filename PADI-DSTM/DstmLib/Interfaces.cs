﻿using System.Collections.Generic;

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


    //Transactionalm server interfaces

    public interface CoordinatorInterface
    {
        bool TxCommit(int tId, List<string> participants);
        bool TxAbort(int tId);


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

    }
}
