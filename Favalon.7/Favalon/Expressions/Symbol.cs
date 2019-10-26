using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public abstract class Symbol : Expression, IEquatable<Symbol?>
    {
        private protected Symbol(Expression higherOrder) =>
            this.HigherOrder = higherOrder;

        public abstract string PrintableName { get; }

        public override Expression HigherOrder { get; }

        public abstract bool Equals(Symbol? other);

        public override bool Equals(Expression? other) =>
            this.Equals(other as Symbol);

        public override string ToString() =>
            $"{this.PrintableName}:{this.HigherOrder}";
    }
}
