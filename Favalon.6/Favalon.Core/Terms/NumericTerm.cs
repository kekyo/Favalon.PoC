using Favalon.Expressions;

namespace Favalon.Terms
{
    public sealed class NumericTerm : LiteralTerm<NumericTerm, string>
    {
        public NumericTerm(string value) :
            base(value)
        { }

        protected override Expression VisitInfer(IInferContext context) =>
            int.TryParse(this.Value, out var intValue) ?
                (Expression)new LiteralExpression<int>(intValue) :
                long.TryParse(this.Value, out var longValue) ?
                    (Expression)new LiteralExpression<long>(longValue) :
            (Expression)new UnknownTermExpression(this);
    }
}
