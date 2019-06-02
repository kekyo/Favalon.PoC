using System;
using System.Collections.Generic;

using TypeInferences.Types.Internals;

namespace TypeInferences.Types
{
    public sealed class AvalonTypeRef : IAvalonType
    {
        internal AvalonType type;

        internal AvalonTypeRef(AvalonType type) =>
            this.type = type;

        public AvalonTypes Type =>
            type.Type;

        public string Identity =>
            this.type.Identity;

        public AvalonType Normalized =>
            this.type;

        public override int GetHashCode() =>
            this.type.GetHashCode();

        public bool Equals(AvalonTypeRef rhs) =>
            object.ReferenceEquals(this, rhs) ?
                true :
                this.type.Equals(rhs.type);

        public bool Equals(IAvalonType rhs) =>
            object.ReferenceEquals(this, rhs) ?
                true :
                this.type.Equals(rhs?.Normalized);

        public override bool Equals(object obj) =>
            obj is IAvalonType rhs ?
                this.Equals(rhs) :
                false;

        public int CompareTo(AvalonTypeRef other) =>
            this.type.CompareTo(other.type);

        public int CompareTo(IAvalonType other) =>
            this.type.CompareTo(other.Normalized);

        public bool IsConvertibleFrom(IAvalonType rhs) =>
            this.type.IsConvertibleFrom(rhs.Normalized);

        public override string ToString() =>
            $"TypeRef: {this.Identity}";

        public IEnumerable<AvalonType> EnumerateTypes() =>
            (this.type is UnionType unionType) ?
                unionType.EnumerateTypes() :
                new[] { this.type };

        public void ComposeToWide(params IAvalonType[] types) =>
            this.type = this.type.ToWide(types);
    }
}
