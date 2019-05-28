using System;
using System.Collections.Generic;
using System.Text;

namespace TypeInferences.Types
{
    public sealed class UnionType : AvalonType
    {
        private readonly IEnumerable<AvalonType> types;

        internal UnionType(IEnumerable<AvalonType> types) =>
            this.types = types;

        public override IEnumerable<AvalonType> EnumerateTypes() =>
            this.types;

        private protected override bool IsConvertibleFrom(AvalonType rhs)
        {
            return false;
        }
    }
}
