using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class KindExpression : Expression
    {
        private KindExpression() :
            base(UndefinedExpression.Instance)
        { }

        public override bool ShowInAnnotation =>
            false;

        protected internal override string FormatReadableString(bool withAnnotation) =>
            "(Kind)";

        public static readonly KindExpression Instance = new KindExpression();
    }
}
