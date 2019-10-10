using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public sealed class Number : Term, IEquatable<Number?>
    {
        public readonly int Value;

        internal Number(int number) =>
            this.Value = number;

        public override Term HigherOrder { get; } =
            Factories.Variable("System.Int32");

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(Number? other) =>
            (other?.Value.Equals(this.Value) ?? false) &&
            (other?.HigherOrder.Equals(this.HigherOrder) ?? false);

        public override bool Equals(Term? other) =>
            this.Equals(other as Number);

        public override string ToString() =>
            this.Value.ToString();
    }
}
