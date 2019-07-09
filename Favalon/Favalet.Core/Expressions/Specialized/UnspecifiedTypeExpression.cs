using Favalet.Expressions.Internals;

namespace Favalet.Expressions.Specialized
{
    public sealed class UnspecifiedTypeExpression : PseudoExpression
    {
        private UnspecifiedTypeExpression() :
            base(KindExpression.Instance)
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (context.FormatNaming == FormatNamings.Strict) ? "(UnspecifiedType)" : "'*";

        internal static readonly UnspecifiedTypeExpression Instance = new UnspecifiedTypeExpression();
    }
}
