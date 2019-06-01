using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypeInferences.Types
{
    public sealed class UnassignedType : AvalonType
    {
        internal UnassignedType() { }

        public override IEnumerable<AvalonType> EnumerateTypes() =>
            Enumerable.Empty<AvalonType>();

        private protected override bool IsConvertibleFrom(AvalonType rhs) =>
            false;
    }
}
