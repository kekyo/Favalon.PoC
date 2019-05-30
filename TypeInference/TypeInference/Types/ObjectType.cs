using System;
using System.Collections.Generic;
using System.Text;

namespace TypeInferences.Types
{
    public sealed class ObjectType : AvalonType
    {
        private ObjectType() { }

        public static readonly AvalonType Instance = new ObjectType();
    }
}
