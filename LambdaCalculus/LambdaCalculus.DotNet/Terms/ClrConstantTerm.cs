using Favalon.Terms.Contexts;
using Favalon.Terms.Logical;
using Favalon.Terms.Methods;
using Favalon.Terms.Types;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Favalon.Terms
{
    public sealed class ClrConstantTerm : ValueTerm<ClrConstantTerm>
    {
        private readonly object value;

        internal ClrConstantTerm(object value, Term higherOrder) :
            base(higherOrder)
        {
            Debug.Assert(value != null);
            Debug.Assert(!(value is bool));

            this.value = value!;
        }

        protected override object GetValue() =>
            this.value;

        protected override Term OnCreate(object value, Term higherOrder) =>
            new ClrConstantTerm(value, higherOrder);

        protected override bool IsIncludeHigherOrderInPrettyPrinting(HigherOrderDetails higherOrderDetail) =>
            higherOrderDetail switch
            {
                HigherOrderDetails.None => false,
                HigherOrderDetails.Full => true,
                _ => !(this.Value.GetType().IsPrimitive() || this.Value is string)
            };

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            this.Value is string str ? $"\"{str}\"" :
            this.Value.ToString();

        public static BooleanTerm From(bool value) =>
            value ? True : False;

        public static Term From(Type type) =>
            ClrTypeTerm.From(type);

        public static Term From(object value) =>
            value switch
            {
                true => True,
                false => False,
                Type type => ClrTypeTerm.From(type),
                MethodInfo method => ClrMethodTerm.From(method),
                _ => new ClrConstantTerm(value, ClrTypeTerm.From(value.GetType()))
            };

        public static readonly BooleanTerm True =
            BooleanTerm.From(true, ClrTypeTerm.From(typeof(bool)));
        public static readonly BooleanTerm False =
            BooleanTerm.From(false, ClrTypeTerm.From(typeof(bool)));
    }
}
