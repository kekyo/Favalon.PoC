﻿using Favalon.Terms.Contexts;

namespace Favalon.Terms.Logical
{
    public sealed class NotTerm : UnaryTerm<NotTerm>
    {
        private NotTerm(Term argument, Term higherOrder) :
            base(argument) =>
            this.HigherOrder = higherOrder;

        public override Term HigherOrder { get; }

        protected override Term OnCreate(Term argument, Term higherOrder) =>
            new NotTerm(argument, higherOrder);

        public override Term Reduce(ReduceContext context)
        {
            var argument = this.Argument.Reduce(context);
            if (argument is BooleanTerm argumentBoolean)
            {
                return BooleanTerm.From(!argumentBoolean.Value, argumentBoolean.HigherOrder);
            }

            var higherOrder = this.HigherOrder.Reduce(context);

            return
                this.Argument.EqualsWithHigherOrder(argument) &&
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    new NotTerm(argument, higherOrder);
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"!{this.Argument.PrettyPrint(context)}";

        public static NotTerm Create(Term argument, Term higherOrder) =>
            new NotTerm(argument, higherOrder);
    }
}
