using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class String : Value, IEquatable<String?>
    {
        private static readonly Value higherOrder =
            Factories.FromType<string>();

        public readonly string Value;

        internal String(string value) =>
            this.Value = value;

        public override Expression HigherOrder =>
            higherOrder;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(String? other) =>
            other?.Value.Equals(this.Value) ?? false;

        public override bool Equals(Value? other) =>
            this.Equals(other as String);

        public override string ToString() =>
            $"\"{this.Value}\"";

        public override Expression Run() =>
            this;
    }
}
