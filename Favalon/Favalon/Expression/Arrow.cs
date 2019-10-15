using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expression
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

        public override Term VisitInfer(Environment environment) =>
            this;

        public override object Reduce() =>
            new Func<object, object>(arg => ((Func<object, object>)arg)(arg)); // TODO: '0 -> '1 -> '0 -> '1

        internal static readonly Arrow Instance =
            new Arrow();
    }
}
