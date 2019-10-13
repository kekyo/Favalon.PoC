using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public sealed class String : Value
    {
        private static readonly Variable higherOrder = new Variable("System.String");

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

        public override Term VisitInfer(Environment environment) =>
            this;

        public override object Reduce() =>
            this.Value;
    }
}
