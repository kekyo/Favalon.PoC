using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class KindExpression : IdentityExpression
    {
        private KindExpression() :
            base(UndefinedExpression.Instance, TextRange.Unknown)
        { }

        public override string Name =>
            "*";

        //public override bool ShowInAnnotation =>
        //    false;

        internal override void SetHigherOrder(Expression higherOrder) =>
            throw new System.InvalidOperationException($"Cannot annotate at kind: *:{higherOrder}");

        //protected internal override string FormatReadableString(FormatContext context) =>
        //    context.StrictNaming ? "(Kind)" : "*";

        public static readonly KindExpression Instance = new KindExpression();
    }
}
