using System;
using System.Collections.Generic;
using System.Text;

namespace TypeInferences.Types
{
    public sealed class UInt16Type : AvalonType
    {
        private UInt16Type() { }

        private protected override bool IsConvertibleFrom(AvalonType rhs)
        {
            return false;
        }

        public static readonly AvalonType Instance = new UInt16Type();
    }
}
