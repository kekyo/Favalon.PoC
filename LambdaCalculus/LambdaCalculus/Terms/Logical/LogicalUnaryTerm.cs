using Favalon.Contexts;

namespace Favalon.Terms.Logical
{
    public abstract class LogicalUnaryTerm : UnaryTerm
    {
        internal LogicalUnaryTerm(Term argument) :
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

    public abstract class LogicalUnaryTerm<T> : LogicalUnaryTerm
        where T : LogicalUnaryTerm
    {
        protected LogicalUnaryTerm(Term argument) :
            base(argument)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T term ?
                this.Argument.Equals(term.Argument) :
                false;
    }
}
