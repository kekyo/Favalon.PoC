using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public sealed class Number : Term, IEquatable<Number?>
    {
        public readonly int Value;

        internal Number(int number)
        {
            this.Value = number;
            this.HigherOrder = Factories.ClrType<int>();
        }

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(Number? other) =>
            other?.Value.Equals(this.Value) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as Number);
    }
}
