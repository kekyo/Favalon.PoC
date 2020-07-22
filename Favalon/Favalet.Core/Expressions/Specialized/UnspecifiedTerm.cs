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
            Instance;

        public bool Equals(UnspecifiedTerm rhs) =>
            rhs != null;

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is UnspecifiedTerm;

        public IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringTypes type) =>
            type switch
            {
                PrettyStringTypes.Simple => "?",
                _ => "(Unspecified)"
            };

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
    }
}
