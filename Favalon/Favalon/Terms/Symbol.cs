using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Terms
{
    public abstract class Symbol : Term, IEquatable<Symbol?>
    {
        private protected Symbol(Term higherOrder) =>
            this.HigherOrder = higherOrder;

        public abstract string PrintableName { get; }

        public override Term HigherOrder { get; }

        public abstract bool Equals(Symbol? other);

        public override bool Equals(Term? other) =>
            this.Equals(other as Symbol);

        public override string ToString() =>
            this.HigherOrder is Unspecified ?
            this.PrintableName :
            $"{this.PrintableName}:{this.HigherOrder}";
    }
}
