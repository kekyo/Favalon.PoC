using Favalon.Expressions;

namespace Favalon.Terms
{
    public sealed class BooleanTerm : LiteralTerm<BooleanTerm, bool>
    {
        public BooleanTerm(bool value) :
            base(value)
        { }

        protected override Expression Visit(InferContext context) =>
            new BooleanExpression(this.Value);
    }
}
