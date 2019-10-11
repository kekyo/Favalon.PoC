using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
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

        public override Term VisitInfer(Environment environment) =>
            this;

        internal static readonly Unspecified Instance =
            new Unspecified();
    }
}
