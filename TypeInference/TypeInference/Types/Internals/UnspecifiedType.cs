using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TypeInferences.Types.Internals
{
    internal sealed class UnspecifiedType : AvalonType
    {
        internal UnspecifiedType() { }

        public override AvalonTypes Type =>
            AvalonTypes.Unspecified;

        public override string Identity =>
            "a";

        public override int GetHashCode() =>
            this.Identity.GetHashCode();

        public override bool Equals(IAvalonType rhs) =>
            // Always false because unspecified type equality props only shared AvalonType instance.
            false;

        public override int CompareTo(IAvalonType other) =>
            -1;

        public override bool IsConvertibleFrom(IAvalonType rhs) =>
            false;
    }
}
