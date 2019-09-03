using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalon.Terms
{
    public sealed class VariableTerm : Term
    {
        public readonly string Value;

        public VariableTerm(string value) =>
            this.Value = value;

        public bool Equals(VariableTerm? rhs) =>
            rhs is VariableTerm term ? this.Value.Equals(term.Value) : false;

        public override bool Equals(Term? other) =>
            this.Equals(other as VariableTerm);
    }
}
