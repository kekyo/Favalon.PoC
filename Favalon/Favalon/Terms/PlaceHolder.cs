using Favalon.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Terms
{
    public sealed class Placeholder : Symbol
    {
        public readonly int Index;

        internal Placeholder(int index, Term higherOrder) :
            base(higherOrder) =>
            this.Index = index;

        public override string PrintableName =>
            $"'{this.Index}";

        public override int GetHashCode() =>
            this.Index.GetHashCode();

        public bool Equals(Placeholder? other) =>
            (other?.Index.Equals(this.Index) ?? false) &&
            (other?.HigherOrder.Equals(this.HigherOrder) ?? false);

        public override bool Equals(Symbol? other) =>
            this.Equals(other as Placeholder);

        protected internal override Expression Visit(Environment environment) =>
            throw new InvalidOperationException(); // TODO:
    }
}
