using Favalon.Contexts;

namespace Favalon.Terms.Logical
{
    public sealed class NotTerm : UnaryTerm<NotTerm>
    {
        private NotTerm(Term argument) :
            base(argument, BooleanTerm.Type)
        { }

        protected override Term OnCreate(Term argument, Term higherOrder) =>
            new NotTerm(argument);

        public override Term Reduce(ReduceContext context)
        {
            var argument = this.Argument.Reduce(context);
            if (argument is BooleanTerm boolArgument)
            {
                return BooleanTerm.From(!boolArgument.Value);
            }

            return
                this.Argument.Equals(argument, true) ?
                    this :
                    new NotTerm(argument);
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"!{this.Argument.PrettyPrint(context)}";

        public static NotTerm Create(Term argument) =>
            new NotTerm(argument);
    }
}
