using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
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

        public override Term VisitInfer(Environment environment) =>
            environment.Lookup(this.Name).FirstOrDefault() ?? this;  // choose overloads
    }
}
