using System;

namespace Favalon.Expression
{
    public abstract class Term : IEquatable<Term?>
    {
        private protected Term()
        { }

        public abstract Term HigherOrder { get; }

        public abstract bool Equals(Term? other);

        public override bool Equals(object obj) =>
            this.Equals(obj as Term);

        public abstract Term VisitInfer(Environment environment);

        public abstract object Reduce();
    }
}
