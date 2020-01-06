using Favalon.Contexts;
using LambdaCalculus.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Terms.Algebric
{
    public sealed class SumTerm : Term, IApplicable
    {
        private SumTerm(Term higherOrder) =>
            this.HigherOrder = higherOrder;

        public override Term HigherOrder { get; }

        public override Term Infer(InferContext context)
        {
            var higherOrder = this.HigherOrder.Infer(context);

            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new SumTerm(higherOrder);
        }

        Term IApplicable.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint)
        {
            var higherOrder = this.HigherOrder.Infer(context);

            // (? -> inferredArgumentHint:? -> ?):higherOrderHint
            var higherOrderFromArgument = LambdaTerm.From(
                inferredArgumentHint.HigherOrder,
                LambdaTerm.From(inferredArgumentHint.HigherOrder, inferredArgumentHint.HigherOrder));

            context.Unify(higherOrder, higherOrderHint);
            context.Unify(higherOrderFromArgument, higherOrderHint);

            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new SumTerm(higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var higherOrder = this.HigherOrder.Fixup(context);

            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new SumTerm(higherOrder);
        }

        Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint)
        {
            var higherOrder = this.HigherOrder.Fixup(context);

            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new SumTerm(higherOrder);
        }

        public override Term Reduce(ReduceContext context)
        {
            var higherOrder = this.HigherOrder.Reduce(context);

            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new SumTerm(higherOrder);
        }

        AppliedResult IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            AppliedResult.Applied(
                new SumClosureTerm(argument, this.HigherOrder.Reduce(context)),
                argument);

        public override bool Equals(Term? other) =>
            other is SumTerm;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "+";

        public static readonly SumTerm Instance =
            new SumTerm(LambdaTerm.Unspecified3);

        public static Term Compose(IEnumerable<Term> terms) =>
            terms.Aggregate((Term)Instance, (agg, term) =>
                ApplyTerm.Create(agg, term, UnspecifiedTerm.Instance));

        private sealed class SumClosureTerm : Term, IApplicable
        {
            private readonly Term argument0;

            public SumClosureTerm(Term argument0, Term higherOrder)
            {
                this.argument0 = argument0;
                this.HigherOrder = higherOrder;
            }

            public override Term HigherOrder { get; }

            public override Term Infer(InferContext context)
            {
                var higherOrder = this.HigherOrder.Infer(context);
                var argument0 = this.argument0.Infer(context);

                context.Unify(argument0.HigherOrder, higherOrder);

                return
                    object.ReferenceEquals(argument0, this.argument0) &&
                    object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                        this :
                        new SumClosureTerm(argument0, higherOrder);
            }

            Term IApplicable.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint)
            {
                var higherOrder = this.HigherOrder.Infer(context);
                var argument0 = this.argument0.Infer(context);

                // ? -> ?
                var higherOrderFromArgument = LambdaTerm.From(
                    argument0.HigherOrder,
                    inferredArgumentHint.HigherOrder);

                context.Unify(higherOrder, higherOrderHint);
                context.Unify(higherOrderFromArgument, higherOrderHint);

                return
                    object.ReferenceEquals(argument0, this.argument0) &&
                    object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                        this :
                        new SumClosureTerm(argument0, higherOrder);
            }

            public override Term Fixup(FixupContext context)
            {
                var higherOrder = this.HigherOrder.Fixup(context);
                var argument0 = this.argument0.Fixup(context);

                return
                    object.ReferenceEquals(argument0, this.argument0) &&
                    object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                        this :
                        new SumClosureTerm(argument0, higherOrder);
            }

            Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint)
            {
                var higherOrder = this.HigherOrder.Fixup(context);
                var argument0 = this.argument0.Fixup(context);

                return
                    object.ReferenceEquals(argument0, this.argument0) &&
                    object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                        this :
                        new SumClosureTerm(argument0, higherOrder);
            }

            public override Term Reduce(ReduceContext context)
            {
                var higherOrder = this.HigherOrder.Reduce(context);
                var argument0 = this.argument0.Reduce(context);

                return
                    object.ReferenceEquals(argument0, this.argument0) &&
                    object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                        this :
                        new SumClosureTerm(argument0, higherOrder);
            }

            internal static AppliedResult Reduce(ReduceContext context, Term argument0, Term argument1)
            {
                var arg0 = argument0.Reduce(context);
                var arg1 = argument1.Reduce(context);


            }

            AppliedResult IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
                Reduce(context, this.argument0, argument);

            public override bool Equals(Term? other) =>
                other is SumTerm;

            protected override string OnPrettyPrint(PrettyPrintContext context) =>
                "+";

            public void Deconstruct(out Term argument0, out Term higherOrder)
            {
                argument0 = this.argument0;
                higherOrder = this.HigherOrder;
            }
        }
    }
}
