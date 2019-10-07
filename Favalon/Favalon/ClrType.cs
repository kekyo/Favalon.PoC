using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public sealed class ClrType : Term, IEquatable<ClrType>
    {
        public readonly Type Type;

        internal ClrType(Type type) =>
            this.Type = type;

        public override int GetHashCode() =>
            this.Type.GetHashCode();

        public bool Equals(ClrType? other) =>
            other?.Type.Equals(this.Type) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as ClrType);
    }
}
