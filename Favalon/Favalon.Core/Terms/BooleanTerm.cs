using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalon.Terms
{
    public sealed class BooleanTerm : Term
    {
        public readonly bool Value;

        public BooleanTerm(bool value) =>
            this.Value = value;

        public bool Equals(BooleanTerm? rhs) =>
            rhs is BooleanTerm term ? this.Value.Equals(term.Value) : false;

        public override bool Equals(Term? other) =>
            this.Equals(other as BooleanTerm);
    }
}
