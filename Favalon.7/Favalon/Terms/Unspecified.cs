using Favalon.Expressions;
using System;

namespace Favalon.Terms
{
    public sealed class Unspecified : Term, IEquatable<Unspecified?>
    {
        private Unspecified()
        { }

        public override Term HigherOrder { get; } =
            null!;

        public override int GetHashCode() =>
            0;

        public bool Equals(Unspecified? other) =>
            other != null;

        public override bool Equals(Term? other) =>
            this.Equals(other as Unspecified);

        public override string ToString() =>
            string.Empty;

        protected internal override Expression Visit(Environment environment) =>
            throw new InvalidOperationException();

        internal static readonly Unspecified Instance =
            new Unspecified();
    }
}
