using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class KindExpression : IdentityExpression
    {
        private KindExpression() :
            base(UndefinedExpression.Instance)
        { }

        public override string Name =>
            "*";

        public override bool ShowInAnnotation =>
            false;

        protected internal override string FormatReadableString(FormatStringContext context) =>
            context.StrictNaming ? "(Kind)" : "*";

        public static readonly KindExpression Instance = new KindExpression();
    }
}
