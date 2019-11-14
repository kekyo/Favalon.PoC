using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class Type : Value, System.IEquatable<Type?>
    {
        internal static readonly Type TypeType =
            new Type(typeof(TypeInfo).GetTypeInfo());

        public readonly TypeInfo Value;

        internal Type(TypeInfo value) =>
            this.Value = value;

        public override Expression HigherOrder =>
            TypeType;

        public override object? RawValue =>
            this.Value;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(Type? other) =>
            other?.Value.Equals(this.Value) ?? false;

        public override bool Equals(Value? other) =>
            this.Equals(other as Type);

        public override string ToString() =>
            this.Value.FullName;

        public override Expression Run() =>
            this;
    }
}
