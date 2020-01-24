using Favalon.Terms.Contexts;

namespace Favalon.Terms.Logical
{
    public sealed class NotTerm : UnaryTerm<NotTerm>
    {
        private NotTerm(Term argument, Term higherOrder) :
            base(argument, higherOrder)
        { }

        protected override Term OnCreate(Term argument, Term higherOrder) =>
            new NotTerm(argument, higherOrder);

        public override Term Reduce(ReduceContext context)
        {
            var higherOrder = this.HigherOrder.Reduce(context);

            var argument = this.Argument.Reduce(context);
            if (argument is BooleanTerm boolArgument)
            {
                return BooleanTerm.From(!boolArgument, higherOrder);
            }

            return
                this.Argument.EqualsWithHigherOrder(argument) &&
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    new NotTerm(argument, higherOrder);
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"!{this.Argument.PrettyPrint(context)}";

        public static NotTerm Create(Term argument, Term higherOrder) =>
            new NotTerm(argument, higherOrder);
    }
}
