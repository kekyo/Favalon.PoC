using System;
using System.Collections.Generic;
using System.Text;

namespace TypeInferences.Types
{
    public sealed class ObjectType : AvalonType
    {
        private ObjectType() { }

        private protected override bool IsConvertibleFrom(AvalonType rhs)
        {
            return true;
        }

        public static readonly AvalonType Instance = new ObjectType();
    }
}
