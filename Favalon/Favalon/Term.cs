using System;

namespace Favalon
{
    public abstract class Term : IEquatable<Term?>
    {
        protected Term()
        { }

        public abstract Term HigherOrder { get; }

        public abstract bool Equals(Term? other);

        public override bool Equals(object obj) =>
            this.Equals(obj as Term);
    }
}
