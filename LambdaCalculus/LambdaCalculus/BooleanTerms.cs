namespace LambdaCalculus
{
    public sealed class BooleanTerm : Term
    {
        internal static readonly Term higherOrder =
            Constant(typeof(bool));

        public readonly bool Value;

        private BooleanTerm(bool value) =>
            this.Value = value;

        public override Term HigherOrder =>
            higherOrder;

        public override Term Reduce(ReduceContext context) =>
            this;

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(InferContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is BooleanTerm rhs ? this.Value.Equals(rhs.Value) : false;

        public static new readonly BooleanTerm True =
            new BooleanTerm(true);
        public static new readonly BooleanTerm False =
            new BooleanTerm(false);
    }

    public sealed class AndOperatorTerm : ApplicableTerm
    {
        private AndOperatorTerm()
        { }

        public override Term HigherOrder =>
            UnspecifiedTerm.Instance;

        public override sealed Term Reduce(ReduceContext context) =>
            this;

        protected internal override Term? ReduceForApply(ReduceContext context, Term rhs) =>
            new AndLeftTerm(rhs.Reduce(context));

        public override Term Infer(InferContext context) =>
            this;

        protected internal override Term? InferForApply(InferContext context, Term rhs) =>
            new AndLeftTerm(rhs.Infer(context));

        public override Term Fixup(InferContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is AndOperatorTerm;

        public static readonly AndOperatorTerm Instance =
            new AndOperatorTerm();

        private sealed class AndLeftTerm : ApplicableTerm
        {
            public readonly Term Lhs;

            public AndLeftTerm(Term lhs) =>
                this.Lhs = lhs;

            public override Term HigherOrder =>
                UnspecifiedTerm.Instance;

            public override Term Reduce(ReduceContext context) =>
                this;

            protected internal override Term? ReduceForApply(ReduceContext context, Term rhs) =>
                AndTerm.Reduce(context, this.Lhs, rhs);

            public override Term Infer(InferContext context) =>
                new AndLeftTerm(this.Lhs.Infer(context));

            protected internal override Term? InferForApply(InferContext context, Term rhs) =>
                AndTerm.Infer(context, this.Lhs, rhs);

            public override Term Fixup(InferContext context) =>
                new AndLeftTerm(this.Lhs.Fixup(context));

            public override bool Equals(Term? other) =>
                other is AndLeftTerm rhs ? this.Lhs.Equals(rhs.Lhs) : false;
        }
    }

    public sealed class AndTerm : Term
    {
        public readonly Term Lhs;
        public readonly Term Rhs;

        internal AndTerm(Term lhs, Term rhs)
        {
            this.Lhs = lhs;
            this.Rhs = rhs;
        }

        public override Term HigherOrder =>
           BooleanTerm.higherOrder;

        internal static Term Reduce(ReduceContext context, Term lhs, Term rhs)
        {
            var lhs_ = lhs.Reduce(context);
            if (lhs_ is BooleanTerm l)
            {
                if (l.Value)
                {
                    var rhs_ = rhs.Reduce(context);
                    if (rhs_ is BooleanTerm r)
                    {
                        return Constant(l.Value && r.Value);
                    }
                    else
                    {
                        return new AndTerm(lhs_, rhs_);
                    }
                }
                else
                {
                    return False();
                }
            }
            else
            {
                return new AndTerm(lhs_, rhs);
            }
        }

        public override sealed Term Reduce(ReduceContext context) =>
            Reduce(context, this.Lhs, this.Rhs);

        internal static Term Infer(InferContext context, Term lhs, Term rhs)
        {
            var lhs_ = lhs.Infer(context);
            var rhs_ = rhs.Infer(context);

            context.Unify(lhs_.HigherOrder, BooleanTerm.higherOrder);
            context.Unify(rhs_.HigherOrder, BooleanTerm.higherOrder);

            return new AndTerm(lhs_, rhs_);
        }

        public override Term Infer(InferContext context) =>
            Infer(context, this.Lhs, this.Rhs);

        public override Term Fixup(InferContext context) =>
            new AndTerm(this.Lhs.Fixup(context), this.Rhs.Fixup(context));

        public override bool Equals(Term? other) =>
            other is AndTerm rhs ?
                (this.Lhs.Equals(rhs.Lhs) && this.Rhs.Equals(rhs.Rhs)) :
                false;
    }

    public sealed class IfOperatorTerm : ApplicableTerm
    {
        private IfOperatorTerm()
        { }

        public override Term HigherOrder =>
           UnspecifiedTerm.Instance;

        public override sealed Term Reduce(ReduceContext context) =>
            this;

        protected internal override Term? ReduceForApply(ReduceContext context, Term rhs) =>
            new ThenTerm(rhs);   // Doesn't reduce at this time.

        public override Term Infer(InferContext context) =>
            this;

        protected internal override Term? InferForApply(InferContext context, Term rhs) =>
            new ThenTerm(rhs.Infer(context));

        public override Term Fixup(InferContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is IfOperatorTerm;

        public static readonly IfOperatorTerm Instance =
            new IfOperatorTerm();

        private sealed class ThenTerm : ApplicableTerm
        {
            public readonly Term Condition;

            public ThenTerm(Term then) =>
                this.Condition = then;

            public override Term HigherOrder =>
                UnspecifiedTerm.Instance;

            public override Term Reduce(ReduceContext context) =>
                new ThenTerm(this.Condition.Reduce(context));

            protected internal override Term? ReduceForApply(ReduceContext context, Term rhs) =>
                new ElseTerm(this.Condition, rhs);   // Doesn't reduce at this time.

            public override Term Infer(InferContext context) =>
                new ThenTerm(this.Condition.Infer(context));

            protected internal override Term? InferForApply(InferContext context, Term rhs) =>
                new ElseTerm(this.Condition.Infer(context), rhs.Infer(context));

            public override Term Fixup(InferContext context) =>
                new ThenTerm(this.Condition.Fixup(context));

            public override bool Equals(Term? other) =>
                other is ThenTerm rhs ? this.Condition.Equals(rhs.Condition) : false;
        }

        private sealed class ElseTerm : ApplicableTerm
        {
            public readonly Term Condition;
            public readonly Term Then;

            public ElseTerm(Term condition, Term then)
            {
                this.Condition = condition;
                this.Then = then;
            }

            public override Term HigherOrder =>
                this.Then.HigherOrder;

            public override Term Reduce(ReduceContext context) =>
                this;  // Cannot reduce then term at this time, because has to examine delayed execution at IfTerm.Reduce().

            protected internal override Term? ReduceForApply(ReduceContext context, Term rhs) =>
                IfTerm.Reduce(context, this.Condition, this.Then, rhs);

            public override Term Infer(InferContext context) =>
                new ElseTerm(this.Condition.Infer(context), this.Then.Infer(context));

            protected internal override Term? InferForApply(InferContext context, Term rhs) =>
                IfTerm.Infer(context, this.Condition, this.Then, rhs);

            public override Term Fixup(InferContext context) =>
                new ElseTerm(this.Condition.Fixup(context), this.Then.Fixup(context));

            public override bool Equals(Term? other) =>
                other is ElseTerm rhs ?
                    (this.Condition.Equals(rhs.Condition) && this.Then.Equals(rhs.Then)) :
                    false;
        }
    }

    public sealed class IfTerm : Term
    {
        public readonly Term Condition;
        public readonly Term Then;
        public readonly Term Else;

        internal IfTerm(Term condition, Term then, Term @else)
        {
            this.Condition = condition;
            this.Then = then;
            this.Else = @else;
        }

        public override Term HigherOrder =>
            (this.Condition is BooleanTerm term) ?
                (term.Value ? this.Then.HigherOrder : this.Else.HigherOrder) :
                this.Then.HigherOrder;  // TODO: Unspecified or OrTypes

        internal static Term Reduce(ReduceContext context, Term condition, Term then, Term @else) =>
            ((BooleanTerm)condition.Reduce(context)).Value ?
                then.Reduce(context) :   // Reduce only then or else term by the conditional.
                @else.Reduce(context);

        public override Term Reduce(ReduceContext context) =>
            Reduce(context, this.Condition, this.Then, this.Else);

        internal static Term Infer(InferContext context, Term condition, Term then, Term @else) =>
            new IfTerm(condition.Infer(context), then.Infer(context), @else.Infer(context));

        public override Term Infer(InferContext context) =>
            Infer(context, this.Condition, this.Then, this.Else);

        public override Term Fixup(InferContext context) =>
            new IfTerm(this.Condition.Fixup(context), this.Then.Fixup(context), this.Else.Fixup(context));

        public override bool Equals(Term? other) =>
            other is IfTerm rhs ?
                (this.Condition.Equals(rhs.Condition) && this.Then.Equals(rhs.Then) && this.Else.Equals(rhs.Else)) :
                false;
    }
}
