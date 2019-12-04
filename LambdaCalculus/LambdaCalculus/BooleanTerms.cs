namespace LambdaCalculus
{
    public sealed class BooleanTerm : Term
    {
        internal static readonly ClrTypeTerm higherOrder =
            Type<bool>();

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

        public void Deconstruct(out bool value) =>
            value = this.Value;

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

        public override Term Infer(InferContext context)
        {
            var lhs = this.Lhs.Infer(context);
            var rhs = this.Rhs.Infer(context);

            context.Unify(lhs.HigherOrder, BooleanTerm.higherOrder);
            context.Unify(rhs.HigherOrder, BooleanTerm.higherOrder);

            return new AndTerm(lhs, rhs);
        }

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
