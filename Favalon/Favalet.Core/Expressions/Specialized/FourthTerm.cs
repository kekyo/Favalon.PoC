using Favalet.Contexts;
using System;

namespace Favalet.Expressions.Specialized
{
    public sealed class FourthTerm :
        Expression, ITerm
    {
        private FourthTerm()
        { }

        public IExpression HigherOrder =>
            null!;

        public bool Equals(FourthTerm rhs) =>
            rhs != null;

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is FourthTerm;

        public IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringTypes type) =>
            type switch
            {
                PrettyStringTypes.Simple => "#",
                _ => "(Fourth)"
            };

        public static readonly FourthTerm Instance =
            new FourthTerm();
    }
}
