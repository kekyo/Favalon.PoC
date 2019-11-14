using Favalon.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Terms
{
    public sealed class String : Value
    {
        private static readonly Variable higherOrder = new Variable(typeof(string).FullName);

        public readonly string Value;

        internal String(string value) =>
            this.Value = value;

        public override Term HigherOrder =>
            higherOrder;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(String? other) =>
            (other?.Value.Equals(this.Value) ?? false) &&
            (other?.HigherOrder.Equals(this.HigherOrder) ?? false);

        public override bool Equals(Value? other) =>
            this.Equals(other as String);

        public override string ToString() =>
            $"\"{this.Value}\"";

        protected internal override Expression Visit(Environment environment) =>
            Expressions.Factories.Value(this.Value);
    }
}
