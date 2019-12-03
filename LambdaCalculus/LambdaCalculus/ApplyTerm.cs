namespace LambdaCalculus
{
    public abstract class ApplicableTerm : Term
    {
        protected internal abstract Term? ReduceForApply(ReduceContext context, Term rhs);
        protected internal abstract Term? InferForApply(InferContext context, Term rhs);
    }

    public sealed class ApplyTerm : Term
    {
        public readonly Term Function;
        public readonly Term Argument;

        internal ApplyTerm(Term function, Term argument)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public override Term HigherOrder =>
            this.Function is ApplicableTerm function ?
                ((LambdaTerm)function.HigherOrder).Body :
                UnspecifiedTerm.Instance;  // TODO: ???

        public override Term Reduce(ReduceContext context)
        {
            var function = this.Function.Reduce(context);

            if (function is ApplicableTerm applicable &&
                applicable.ReduceForApply(context, this.Argument) is Term term)
            {
                return term;
            }
            else
            {
                return new ApplyTerm(function, this.Argument.Reduce(context));
            }
        }

        public override Term Infer(InferContext context)
        {
            var function = this.Function.Infer(context);

            if (function is ApplicableTerm applicable &&
                applicable.InferForApply(context, this.Argument) is Term term)
            {
                return new ApplyTerm(term, this.Argument.Infer(context));
            }
            else
            {
                return new ApplyTerm(function, this.Argument.Infer(context));
            }
        }

        public override Term Fixup(InferContext context) =>
            new ApplyTerm(this.Function.Fixup(context), this.Argument.Fixup(context));

        public override bool Equals(Term? other) =>
            other is ApplyTerm rhs ?
                (this.Function.Equals(rhs.Function) && this.Argument.Equals(rhs.Argument)) :
                false;
    }
}
