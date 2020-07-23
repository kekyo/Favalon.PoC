using Favalet.Contexts;
using System;

namespace Favalet.Expressions.Specialized
{
    public sealed class UnspecifiedTerm :
        Expression, ITerm
    {
        private UnspecifiedTerm()
        { }

        public IExpression HigherOrder =>
            null!;

        public bool Equals(UnspecifiedTerm rhs) =>
            rhs != null;

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is UnspecifiedTerm;

        public IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringContext context) =>
            this.FinalizePrettyString(
                context,
                context.IsSimple ?
                    "?" :
                    "Unspecified");

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
    }
}
