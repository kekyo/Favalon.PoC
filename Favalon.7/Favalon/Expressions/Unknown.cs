using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class Unknown : Expression
    {
        public readonly Terms.Term Term;

        internal Unknown(Terms.Term term) =>
            this.Term = term;

        public override Expression HigherOrder =>
            null!;

        public bool Equals(Unknown? other) =>
            other?.Term.Equals(this.Term) ?? false;

        public override bool Equals(Expression? other) =>
            this.Equals(other as Unknown);

        public override Expression Run() =>
            this;
    }
}
