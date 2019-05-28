using System;
using System.Collections.Generic;
using System.Text;

namespace TypeInferences.Types
{
    public sealed class StringType : AvalonType
    {
        private StringType() { }

        public static readonly AvalonType Instance = new StringType();
    }
}
