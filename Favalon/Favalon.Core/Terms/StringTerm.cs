using Favalon.Expressions;

namespace Favalon.Terms
{
    public sealed class StringTerm : LiteralTerm<StringTerm, string>
    {
        public StringTerm(string value) :
            base(value)
        { }

        protected override Expression VisitInfer(IInferContext context) =>
            new StringExpression(this.Value);

        public override string ToString() =>
            $"{this.GetType().Name}: \"{this.Value}\"";
    }
}
