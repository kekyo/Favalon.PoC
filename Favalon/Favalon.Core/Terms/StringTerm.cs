using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalon.Terms
{
    public sealed class StringTerm : Term
    {
        public readonly string Value;

        public StringTerm(string value) =>
            this.Value = value;

        public bool Equals(StringTerm? rhs) =>
            rhs is StringTerm term ? this.Value.Equals(term.Value) : false;

        public override bool Equals(Term? other) =>
            this.Equals(other as StringTerm);
    }
}
