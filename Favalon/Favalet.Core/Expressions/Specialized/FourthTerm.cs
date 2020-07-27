using Favalet.Contexts;
using System;

namespace Favalet.Expressions.Specialized
{
    public sealed class FourthTerm :
        Expression, ITerm
    {
        private FourthTerm()
        { }

        public override IExpression HigherOrder =>
            null!;

        public bool Equals(FourthTerm rhs) =>
            rhs != null;

        public override bool Equals(IExpression? other) =>
            other is FourthTerm;

        public override IExpression Infer(IReduceContext context) =>
            this;

        public override IExpression Fixup(IReduceContext context) =>
            this;

        public override IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    "#" :
                    "Fourth");

        public static readonly FourthTerm Instance =
            new FourthTerm();
    }
}
