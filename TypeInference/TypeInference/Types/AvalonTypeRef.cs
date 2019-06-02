using System;
using System.Collections.Generic;

using TypeInferences.Types.Internals;

namespace TypeInferences.Types
{
    public sealed class AvalonTypeRef :
        AvalonType, IEquatable<AvalonTypeRef>, IComparable<AvalonTypeRef>
    {
        internal AvalonType type;

        internal AvalonTypeRef(AvalonType type) =>
            this.type = type;

        public override AvalonTypes Type =>
            type.Type;

        public override string Identity =>
            this.type.Identity;

        public override int GetHashCode() =>
            this.type.GetHashCode();

        public bool Equals(AvalonTypeRef rhs) =>
            object.ReferenceEquals(this, rhs) ?
                true :
                this.type.Equals(rhs.type);

        public override bool Equals(AvalonType rhs) =>
            object.ReferenceEquals(this, rhs) ?
                true :
                this.type.Equals(rhs?.Normalized);

        public override bool Equals(object obj) =>
            obj is AvalonType rhs ?
                this.Equals(rhs) :
                false;

        public int CompareTo(AvalonTypeRef other) =>
            this.type.CompareTo(other.type);

        public override int CompareTo(AvalonType other) =>
            this.type.CompareTo(other.Normalized);

        internal override bool IsConvertibleFrom(AvalonType rhs) =>
            this.type.IsConvertibleFrom(rhs.Normalized);

        public override string ToString() =>
            $"TypeRef: {this.Identity}";

        public IEnumerable<AvalonType> EnumerateTypes() =>
            (this.type is UnionType unionType) ?
                unionType.EnumerateTypes() :
                new[] { this.type };

        public void ComposeToWide(params AvalonType[] types) =>
            this.type = this.type.ToWide(types);
    }
}
