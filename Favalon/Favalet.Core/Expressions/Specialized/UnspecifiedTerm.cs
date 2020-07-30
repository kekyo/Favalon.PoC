using Favalet.Contexts;
using System;
using System.Diagnostics;

namespace Favalet.Expressions.Specialized
{
    public sealed class UnspecifiedTerm :
        Expression, ITerminationTerm
    {
        private UnspecifiedTerm()
        { }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override IExpression HigherOrder =>
            null!;

        public bool Equals(UnspecifiedTerm rhs) =>
            rhs != null;

        public override bool Equals(IExpression? other) =>
            other is UnspecifiedTerm;

        protected override IExpression Infer(IReduceContext context) =>
            context is IPlaceholderProvider provider ?
                (IExpression)provider.CreatePlaceholder(PlaceholderOrderHints.TypeOrAbove) :
                this;

        protected override IExpression Fixup(IReduceContext context) =>
            this;

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        protected override string GetPrettyString(IPrettyStringContext context) =>
            "_";

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
    }
}
