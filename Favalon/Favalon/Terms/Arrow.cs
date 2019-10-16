using Favalon.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Terms
{
    public sealed class Arrow : Symbol
    {
        private Arrow() :
            base(Unspecified.Instance)
        { }

        public override string PrintableName =>
            "->";

        public override int GetHashCode() =>
            0;

        public bool Equals(Arrow? other) =>
            other != null;

        public override bool Equals(Symbol? other) =>
            this.Equals(other as Arrow);

        public override Expression VisitInfer(Environment environment) =>
            throw new InvalidOperationException();

        internal static readonly Arrow Instance =
            new Arrow();
    }
}
