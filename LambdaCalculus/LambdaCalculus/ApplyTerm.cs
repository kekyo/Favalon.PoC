namespace Favalon
{
    public interface IApplicable
    {
        Term? ReduceForApply(ReduceContext context, Term rhs);
    }

    public sealed class ApplyTerm : Term
    {
        public readonly Term Function;
        public readonly Term Argument;

        internal ApplyTerm(Term function, Term argument, Term higherOrder)
        {
            this.Function = function;
            this.Argument = argument;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        public override Term Infer(InferContext context)
        {
            var function = this.Function.Infer(context);
            var argument = this.Argument.Infer(context);
            var higherOrder = this.HigherOrder.Infer(context);

            // (f:('1 -> '2) a:'1):'2
            context.Unify(function.HigherOrder, new LambdaTerm(argument.HigherOrder, higherOrder));

            return new ApplyTerm(function, argument, higherOrder);
        }

        public override Term Fixup(FixupContext context) =>
            new ApplyTerm(
                this.Function.Fixup(context),
                this.Argument.Fixup(context),
                this.HigherOrder.Fixup(context));

        public override Term Reduce(ReduceContext context)
        {
            var function = this.Function.Reduce(context);

            if (function is IApplicable applicable &&
                applicable.ReduceForApply(context, this.Argument) is Term term)
            {
                return term;
            }
            else
            {
                return new ApplyTerm(
                    function,
                    this.Argument.Reduce(context),
                    this.HigherOrder.Reduce(context));
            }
        }

        public override bool Equals(Term? other) =>
            other is ApplyTerm rhs ?
                (this.Function.Equals(rhs.Function) && this.Argument.Equals(rhs.Argument)) :
                false;

        public override int GetHashCode() =>
            this.Function.GetHashCode() ^ this.Argument.GetHashCode();
    }
}
