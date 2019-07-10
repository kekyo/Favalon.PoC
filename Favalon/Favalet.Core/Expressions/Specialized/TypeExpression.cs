using Favalet.Expressions.Internals;

namespace Favalet.Expressions.Specialized
{
    public sealed class TypeExpression : PseudoExpression
    {
        private TypeExpression() :
            base(KindExpression.Instance)
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (context.FormatNaming == FormatNamings.Strict) ? "(Type)" : "?";

        internal static readonly TypeExpression Instance =
            new TypeExpression();
    }
}
