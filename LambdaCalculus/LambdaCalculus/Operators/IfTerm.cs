namespace LambdaCalculus.Operators
{
    public sealed class IfOperatorTerm : OperatorSymbolTerm<IfOperatorTerm>, IApplicable
    {
        private static readonly Term higherOrder =
            new LambdaTerm(BooleanTerm.Type, LambdaTerm.Unspecified);

        private IfOperatorTerm()
        { }

        public override Term HigherOrder =>
            higherOrder;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
            new ConditionTerm(rhs);

        public static readonly IfOperatorTerm Instance =
            new IfOperatorTerm();

        private sealed class ConditionTerm : OperatorArgument0Term<ConditionTerm>, IApplicable
        {
            private static readonly Term higherOrder =
                new LambdaTerm(
                    LambdaCalculus.UnspecifiedTerm.Instance,
                    LambdaTerm.Unspecified);

            public ConditionTerm(Term condition) :
                base(condition)
            { }

            public override Term HigherOrder =>
                higherOrder;

            protected override Term Create(Term argument) =>
                new ConditionTerm(argument);

            Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
                new ThenTerm(this.Argument0, rhs);
        }

        private sealed class ThenTerm : OperatorArgument1Term<ThenTerm>, IApplicable
        {
            public ThenTerm(Term condition, Term then) :
                base(condition, then)
            { }

            public override Term HigherOrder =>
                new LambdaTerm(this.Argument1.HigherOrder, this.Argument1.HigherOrder);

            protected override Term Create(Term argument0, Term argument1) =>
                new ThenTerm(argument0, argument1);

            Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
                IfTerm.Reduce(context, this.Argument0, this.Argument1, rhs);
        }
    }

    public sealed class IfTerm : Term
    {
        public readonly Term Condition;
        public readonly Term Then;
        public readonly Term Else;

        internal IfTerm(Term condition, Term then, Term @else, Term higherOrder)
        {
            this.Condition = condition;
            this.Then = then;
            this.Else = @else;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        internal static Term Reduce(ReduceContext context, Term condition, Term then, Term @else) =>
            ((BooleanTerm)condition.Reduce(context)).Value ?
                then.Reduce(context) :   // Reduce only then or else term by the conditional.
                @else.Reduce(context);

        public override Term Reduce(ReduceContext context) =>
            Reduce(context, this.Condition, this.Then, this.Else);

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

        public override Term Fixup(InferContext context) =>
            new IfTerm(
                this.Condition.Fixup(context),
                this.Then.Fixup(context),
                this.Else.Fixup(context),
                this.HigherOrder.Fixup(context));

        public override bool Equals(Term? other) =>
            other is IfTerm rhs ?
                (this.Condition.Equals(rhs.Condition) && this.Then.Equals(rhs.Then) && this.Else.Equals(rhs.Else)) :
                false;
    }
}
