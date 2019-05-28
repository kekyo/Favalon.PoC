using System;
using System.Collections.Generic;
using System.Text;

namespace TypeInferences.Types
{
    public class DoubleType : AvalonType
    {
        private protected DoubleType() { }

        private protected override bool IsConvertibleFrom(AvalonType rhs)
        {
            if (rhs is Int32Type)
            {
                return true;
            }
            if (rhs is UInt16Type)
            {
                return true;
            }
            return false;
        }

        public static readonly AvalonType Instance = new DoubleType();
    }
}
