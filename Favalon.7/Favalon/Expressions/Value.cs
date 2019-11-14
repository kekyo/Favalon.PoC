using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public abstract class Value : Expression, IEquatable<Value?>
    {
        private protected Value()
        {
        }

        public abstract object? RawValue { get; }

        public abstract bool Equals(Value? other);

        public override bool Equals(Expression? other) =>
            this.Equals(other as Value);
    }
}
