using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DstmLib
{
    public interface MasterInterface
    {

        PadInt CreatePadInt(int uid);

        PadInt AccessPadInt(int uid);

    }

    
}
