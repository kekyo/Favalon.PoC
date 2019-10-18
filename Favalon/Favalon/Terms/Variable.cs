using Favalon.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Terms
{
    public sealed class Variable : Symbol
    {
        public readonly string Name;

        internal Variable(string name) :
            this(name, Unspecified.Instance)
        { }

        internal Variable(string name, Term higherOrder) :
            base(higherOrder) =>
            this.Name = name;

        public override string PrintableName =>
            this.Name;

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public bool Equals(Variable? other) =>
            (other?.Name.Equals(this.Name) ?? false) &&
            (other?.HigherOrder.Equals(this.HigherOrder) ?? false);

        public override bool Equals(Symbol? other) =>
            this.Equals(other as Variable);

        protected internal override Expression Visit(Environment environment)
        {
            var terms = environment.Lookup(this.Name);
            return terms.FirstOrDefault()?.Visit(environment) ??  // TODO: choice overloads.
                Expressions.Factories.Unknown(this);
        }
    }
}
