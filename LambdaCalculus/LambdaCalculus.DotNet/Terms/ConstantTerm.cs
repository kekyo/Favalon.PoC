using Favalon.Terms.Contexts;
using Favalon.Terms.Logical;
using Favalon.Terms.Types;
using System;

namespace Favalon.Terms
{
    public sealed class ConstantTerm : ValueTerm<ConstantTerm>
    {
        internal static readonly Term ClrBooleanType =
            ClrTypeTerm.From(typeof(bool));

        private static readonly BooleanTerm trueTerm =
            new ClrTrueTerm();
        private static readonly BooleanTerm falseTerm =
            new ClrFalseTerm();

        private readonly object value;

        internal ConstantTerm(object value, Term higherOrder) :
            base(higherOrder) =>
            this.value = value;

        protected override object GetValue() =>
            this.value;

        protected override Term OnCreate(object value, Term higherOrder) =>
            new ConstantTerm(value, higherOrder);

        protected override bool IsIncludeHigherOrderInPrettyPrinting(HigherOrderDetails higherOrderDetail) =>
            higherOrderDetail switch
            {
                HigherOrderDetails.None => false,
                HigherOrderDetails.Full => true,
                _ => this.Value is string || this.Value is char
            };

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            this.Value is string str ? $"\"{str}\"" :
            this.Value.ToString();

        public static BooleanTerm From(bool value) =>
            value ? trueTerm : falseTerm;

        public static Term From(Type type) =>
            ClrTypeTerm.From(type);

        public static Term From(object value) =>
            value switch
            {
                true => trueTerm,
                false => falseTerm,
                Type type => ClrTypeTerm.From(type),
                _ => new ConstantTerm(value, ClrTypeTerm.From(value.GetType()))
            };
    }
}
