using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalon.Terms
{
    public sealed class NumericTerm : Term
    {
        public readonly string Value;

        public NumericTerm(string value) =>
            this.Value = value;

        public bool Equals(NumericTerm? rhs) =>
            rhs is NumericTerm term ? this.Value.Equals(term.Value) : false;

        public override bool Equals(Term? other) =>
            this.Equals(other as NumericTerm);
    }
}
