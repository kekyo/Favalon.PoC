using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public struct Unit
    {
        public override string ToString() =>
            "()";

        public static readonly Unit Value;
    }
}
