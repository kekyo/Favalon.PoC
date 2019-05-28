using System;
using System.Collections.Generic;
using System.Text;

namespace TypeInferences.Types
{
    public sealed class Int32Type : AvalonType
    {
        private Int32Type() { }

        private protected override bool IsConvertibleFrom(AvalonType rhs)
        {
            if (rhs is UInt16Type)
            {
                return true;
            }

            return false;
        }

        public static new readonly AvalonType Instance = new Int32Type();
    }
}
