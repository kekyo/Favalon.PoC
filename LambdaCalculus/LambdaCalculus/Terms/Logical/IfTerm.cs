using Favalon.Contexts;

namespace Favalon.Terms.Logical
{
    public sealed class IfTerm : HigherOrderHoldTerm
    {
        public readonly Term Condition;
        public readonly Term Then;
        public readonly Term Else;

        internal IfTerm(Term condition, Term then, Term @else, Term higherOrder) :
            base(higherOrder)
        {
            this.Condition = condition;
            this.Then = then;
            this.Else = @else;
        }

        public override Term Infer(InferContext context)
        {
            var condition = this.Condition.Infer(context);
            var then = this.Then.Infer(context);
            var @else = this.Else.Infer(context);
            var higherOrder = this.HigherOrder.Infer(context);

            context.Unify(condition.HigherOrder, BooleanTerm.Type);

            if (condition is BooleanTerm(bool value))
            {
                if (value)
                {
                    context.Unify(higherOrder, then.HigherOrder);
                }
                else
                {
                    context.Unify(higherOrder, @else.HigherOrder);
                }
            }
            else
            {
                context.Unify(higherOrder, then.HigherOrder);
            }

            return new IfTerm(condition, then, @else, higherOrder);
        }

        public override Term Fixup(FixupContext context) =>
            new IfTerm(
                this.Condition.Fixup(context),
                this.Then.Fixup(context),
                this.Else.Fixup(context),
                this.HigherOrder.Fixup(context));

        internal static Term Reduce(ReduceContext context, Term condition, Term then, Term @else) =>
            ((BooleanTerm)condition.Reduce(context)).Value ?
                then.Reduce(context) :   // Reduce only then or else term by the conditional.
                @else.Reduce(context);

        public override Term Reduce(ReduceContext context) =>
            Reduce(context, this.Condition, this.Then, this.Else);

        public override bool Equals(Term? other) =>
            other is IfTerm rhs ?
                (this.Condition.Equals(rhs.Condition) && this.Then.Equals(rhs.Then) && this.Else.Equals(rhs.Else)) :
                false;

        public override int GetHashCode() =>
            this.Condition.GetHashCode() ^ this.Then.GetHashCode() ^ this.Else.GetHashCode();
    }
}
