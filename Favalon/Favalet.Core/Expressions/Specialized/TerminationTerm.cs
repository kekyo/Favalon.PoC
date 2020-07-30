using Favalet.Contexts;
using System;
using System.Diagnostics;

namespace Favalet.Expressions.Specialized
{
    internal sealed class TerminationTerm :
        Expression, ITerm
    {
        private TerminationTerm()
        { }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override IExpression HigherOrder =>
            Instance;

        public bool Equals(FourthTerm rhs) =>
            false;

        public override bool Equals(IExpression? other) =>
            false;

        protected override IExpression Infer(IReduceContext context) =>
            this;

        protected override IExpression Fixup(IReduceContext context) =>
            this;

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        protected override string GetPrettyString(IPrettyStringContext context) =>
            "#TERM";

        public static readonly TerminationTerm Instance =
            new TerminationTerm();
    }
}
