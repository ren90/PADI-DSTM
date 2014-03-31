using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSTMLib
{
    public static class DSTMLib
    {
        // methods for manipulating PADI-DSTM

        public static bool init() { throw new NotImplementedException();}

        public static bool TxBegin() { throw new NotImplementedException(); }

        public static bool TxCommit() { throw new NotImplementedException(); }

        public static bool TxAbort() { throw new NotImplementedException(); }

        public static bool Status() { throw new NotImplementedException(); }

        public static bool Fail(string URL) { throw new NotImplementedException(); }

        public static bool Freeze(string URL) { throw new NotImplementedException(); }

        public static bool Recover(string URL) { throw new NotImplementedException(); }

        // methods for creating and accessing PADInts

        public static PADInt CreatePADInt(int uid) { throw new NotImplementedException(); }

        public static PADInt AccessPADInt(int uid) { throw new NotImplementedException(); }

        
    }
}
