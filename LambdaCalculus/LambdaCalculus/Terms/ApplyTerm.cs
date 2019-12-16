using Favalon.Contexts;

namespace Favalon.Terms
{
    // It's only using in ApplyTerm.
    public interface IApplicable
    {
        Term InferForApply(InferContext context, Term inferredArgument);
        Term? ReduceForApply(ReduceContext context, Term argument);
    }

    public sealed class ApplyTerm : HigherOrderHoldTerm
    {
        public readonly Term Function;
        public readonly Term Argument;

        internal ApplyTerm(Term function, Term argument, Term higherOrder) :
            base(higherOrder)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public override Term Infer(InferContext context)
        {
            var argument = this.Argument.Infer(context);

            var function = (this.Function is IApplicable applicable) ?
                applicable.InferForApply(context, argument) :
                this.Function.Infer(context);

            var higherOrder = this.HigherOrder.Infer(context);

            // (f:('1 -> '2) a:'1):'2
            context.Unify(
                function.HigherOrder,
                LambdaTerm.Create(argument.HigherOrder, higherOrder));

            return
                object.ReferenceEquals(function, this.Function) &&
                object.ReferenceEquals(argument, this.Argument) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                    this :
                    new ApplyTerm(function, argument, higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var argument = this.Argument.Fixup(context);
            var function = this.Function.Fixup(context);
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

            var argument = this.Argument.Reduce(context);   // TODO: Reduced twice
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
