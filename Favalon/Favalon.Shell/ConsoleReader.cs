using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalon
{
    internal sealed class ConsoleReader
    {
        public static char? Read()
        {
            var stdin = NativeMethods.GetStdHandle(NativeMethods.STD_INPUT_HANDLE);

            if (NativeMethods.ReadConsoleInput(stdin) is NativeMethods.KEY_EVENT_RECORD record)
            {
                if (record.bKeyDown)
                {
                    return record.UnicodeChar;
                }
            }
            return null;
        }
    }
}
