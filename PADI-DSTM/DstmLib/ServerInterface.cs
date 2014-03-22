using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DstmLib
{
    public interface ServerInterface
    {

        int Read();
        void Write(int value);

    }
}
