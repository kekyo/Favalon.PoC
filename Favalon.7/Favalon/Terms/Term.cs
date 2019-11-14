using Favalon.Expressions;
using System;

namespace Favalon.Terms
{
#pragma warning disable CS0659
    public abstract class Term : IEquatable<Term?>
#pragma warning restore CS0659
    {
        private protected Term()
        { }

        public abstract Term HigherOrder { get; }

        public abstract bool Equals(Term? other);

        public override bool Equals(object obj) =>
            this.Equals(obj as Term);

        protected internal abstract Expression Visit(Environment environment);
    }
}
