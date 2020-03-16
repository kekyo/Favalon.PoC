using Favalon.Terms.Contexts;

namespace Favalon.Terms.Logical
{
    public sealed class BooleanTerm : Term, IIdentityTerm, IValueTerm
    {
        public readonly bool Value;

        private BooleanTerm(bool value, Term higherOrder)
        {
            this.Value = value;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        object IValueTerm.Value =>
            this.Value;

        public string Identity =>
            this.Value ? "true" : "false";

        public override Term Infer(InferContext context)
        {
            var higherOrder = context.ResolveHigherOrder(this.HigherOrder);

            if (context.LookupBoundTerm(this.Identity) is Term bound)
            {
                context.Unify(bound.HigherOrder, higherOrder);
                return From(Value, higherOrder);
            }

            return From(Value, higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var higherOrder = this.HigherOrder.Fixup(context);
            return From(Value, higherOrder);
        }

        public override Term Reduce(ReduceContext context)
        {
            if (context.LookupBoundTerm(this.Identity) is Term bound)
            {
                // Ignore repeating self references (will cause stack overflow)
                return bound is IIdentityTerm ? bound : bound.Reduce(context);
            }

            var higherOrder = this.HigherOrder.Reduce(context);
            return From(Value, higherOrder);
        }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is BooleanTerm rhs ? (Value == rhs.Value) : false;

        protected override bool IsIncludeHigherOrderInPrettyPrinting(HigherOrderDetails higherOrderDetail) =>
            false;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            this.Identity;

        public static BooleanTerm From(bool value, Term higherOrder) =>
            higherOrder.Equals(Type) ? From(value) : new BooleanTerm(value, higherOrder);

        public static BooleanTerm From(bool value) =>
            value ? True : False;

        public static readonly Term Type =
            FreeVariableTerm.Create("bool", UnspecifiedTerm.Instance);

        public static readonly BooleanTerm True =
            new BooleanTerm(true, Type);
        public static readonly BooleanTerm False =
            new BooleanTerm(false, Type);
    }
}
