using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public sealed class Number : Value
    {
        public readonly string Value;

        internal Number(string number) =>
            this.Value = number;

        public override Term HigherOrder =>
            Unspecified.Instance;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(Number? other) =>
            other?.Value.Equals(this.Value) ?? false;

        public override bool Equals(Value? other) =>
            this.Equals(other as Number);

        public override string ToString() =>
            this.Value.ToString();

        public override Term VisitInfer(Environment environment)
        {
            if (int.TryParse(this.Value, out var intValue))
            {
                return Factories.Number(intValue);
            }
            else if (double.TryParse(this.Value, out var doubleValue))
            {
                return Factories.Number(doubleValue);
            }
            else
            {
                throw new FormatException();
            }
        }

        public override object Reduce() =>
            throw new InvalidOperationException();
    }

    public sealed class Number<T> : Value
        where T : struct
    {
        private static readonly Term higherOrder = new Variable(typeof(T).FullName);

        public readonly T Value;

        internal Number(T number) =>
            this.Value = number;

        public override Term HigherOrder =>
            higherOrder;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(Number<T>? other) =>
            other?.Value.Equals(this.Value) ?? false;

        public override bool Equals(Value? other) =>
            this.Equals(other as Number<T>);

        public override string ToString() =>
            this.Value.ToString();

        public override Term VisitInfer(Environment environment) =>
            this;

        public override object Reduce() =>
            this.Value;
    }
}
