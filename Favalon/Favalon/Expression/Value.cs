using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expression
{
    public abstract class Value : Term, IEquatable<Value?>
    {
        private protected Value()
        { }

        public abstract bool Equals(Value? other);

        public override bool Equals(Term? other) =>
            this.Equals(other as Value);
    }
}
