using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class Number<T> : Value, IEquatable<Number<T>?>
        where T : struct
    {
        private static readonly Expression higherOrder =
            Factories.FromType<T>();

        public readonly T Value;

        internal Number(T number) =>
            this.Value = number;

        public override Expression HigherOrder =>
            higherOrder;

        public override object? RawValue =>
            this.Value;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(Number<T>? other) =>
            other?.Value.Equals(this.Value) ?? false;

        public override bool Equals(Value? other) =>
            this.Equals(other as Number<T>);

        public override string ToString() =>
            this.Value.ToString();

        public override Expression Run() =>
            this;
    }
}
