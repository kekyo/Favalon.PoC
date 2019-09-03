using System;

namespace Favalon.Terms
{
    public abstract class Term : IEquatable<Term?>
    {
        protected Term()
        { }

        public abstract bool Equals(Term? rhs);

        public override bool Equals(object? rhs) =>
            this.Equals(rhs as Term);
    }
}
