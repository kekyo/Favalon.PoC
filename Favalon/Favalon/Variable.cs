using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public sealed class Variable : Term, IEquatable<Variable?>
    {
        public readonly string Name;

        internal Variable(string name) =>
            this.Name = name;

        private Term higherOrder =
            Unspecified.Instance;

        public override Term HigherOrder =>
            this.higherOrder;

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public bool Equals(Variable? other) =>
            (other?.Name.Equals(this.Name) ?? false) &&
            (other?.HigherOrder.Equals(this.HigherOrder) ?? false);

        public override bool Equals(Term? other) =>
            this.Equals(other as Variable);

        public override string ToString() =>
            this.HigherOrder is Unspecified ?
            this.Name :
            $"{this.Name}:{this.HigherOrder}";
    }
}
