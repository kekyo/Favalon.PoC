using Favalet.Contexts;
using System;
using System.Diagnostics;

namespace Favalet.Expressions.Specialized
{
    public sealed class FourthTerm :
        Expression, ITerminationTerm
    {
        private FourthTerm()
        { }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override IExpression HigherOrder =>
            null!;

        public bool Equals(FourthTerm rhs) =>
            rhs != null;

        public override bool Equals(IExpression? other) =>
            other is FourthTerm;

        protected override IExpression Infer(IReduceContext context) =>
            this;

        protected override IExpression Fixup(IReduceContext context) =>
            this;

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        protected override string GetPrettyString(IPrettyStringContext context) =>
            "#";

        public static readonly FourthTerm Instance =
            new FourthTerm();
    }
}
