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
            context.Unify(
                function.HigherOrder,
                new LambdaTerm(argument.HigherOrder, higherOrder));

            return
                object.ReferenceEquals(function, this.Function) &&
                object.ReferenceEquals(argument, this.Argument) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new ApplyTerm(function, argument, higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var function = this.Function.Fixup(context);
            var argument = this.Argument.Fixup(context);
            var higherOrder = this.HigherOrder.Fixup(context);

            return
                object.ReferenceEquals(function, this.Function) &&
                object.ReferenceEquals(argument, this.Argument) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new ApplyTerm(function, argument, higherOrder);
        }

        public override Term Reduce(ReduceContext context)
        {
            var function = this.Function.Reduce(context);

            if (function is IApplicable applicable &&
                applicable.ReduceForApply(context, this.Argument) is Term term)
            {
                return term;
            }

            var argument = this.Argument.Reduce(context);
            var higherOrder = this.HigherOrder.Reduce(context);

            return
                object.ReferenceEquals(function, this.Function) &&
                object.ReferenceEquals(argument, this.Argument) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new ApplyTerm(function, argument, higherOrder);
        }

        public override bool Equals(Term? other) =>
            other is ApplyTerm rhs ?
                (this.Function.Equals(rhs.Function) && this.Argument.Equals(rhs.Argument)) :
                false;

        public override int GetHashCode() =>
            this.Function.GetHashCode() ^ this.Argument.GetHashCode();
    }
}
