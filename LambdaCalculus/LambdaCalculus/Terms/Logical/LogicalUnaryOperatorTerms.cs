using Favalon.Contexts;

namespace Favalon.Terms.Logical
{
    public abstract class LogicalUnaryOperatorTerm : UnaryTerm
    {
        internal LogicalUnaryOperatorTerm(Term argument) :
            base(argument)
        { }

        public override sealed Term HigherOrder =>
            BooleanTerm.Type;

        protected override sealed Term Infer(InferContext context, Term argument)
        {
            context.Unify(argument.HigherOrder, BooleanTerm.Type);

            return this.Create(argument);
        }
    }

    public abstract class LogicalUnaryOperatorTerm<T> : LogicalUnaryOperatorTerm
        where T : LogicalUnaryOperatorTerm
    {
        protected LogicalUnaryOperatorTerm(Term argument) :
            base(argument)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T term ?
                this.Argument.Equals(term.Argument) :
                false;
    }
}
