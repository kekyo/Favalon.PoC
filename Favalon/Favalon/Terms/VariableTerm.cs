using System;

namespace Favalon.Terms
{
    public sealed class VariableTerm : Term
    {
        public readonly string Name;

        internal VariableTerm(string name) =>
            this.Name = name;

        public override bool Reducible =>
            false;

        public override Term VisitReplace(string name, Term term) =>
            (name == this.Name) ?
                term :
                this;

        public override Term VisitReduce() =>
            this;

        public override string ToString() =>
            this.Name;
    }
}
