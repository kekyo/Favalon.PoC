using Favalon.Contexts;

namespace Favalon.Terms.Logical
{
    public sealed class NotTerm : LogicalUnaryOperatorTerm<NotTerm>
    {
        internal NotTerm(Term argument) :
            base(argument)
        { }

        protected override Term Create(Term argument) =>
            new NotTerm(argument);

        internal static Term Reduce(ReduceContext context, Term argument)
        {
            var argument_ = argument.Reduce(context);
            if (argument_ is BooleanTerm a)
            {
                return BooleanTerm.From(!a.Value);
            }
            else
            {
                return new NotTerm(argument_);
            }
        }

        public override Term Reduce(ReduceContext context) =>
            Reduce(context, this.Argument);
    }
}
