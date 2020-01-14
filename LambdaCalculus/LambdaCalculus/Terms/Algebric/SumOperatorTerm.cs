using Favalon.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Terms.Algebric
{
    public sealed class SumOperatorTerm<TSumTerm> : Term, IApplicable
        where TSumTerm : BinaryTerm, new()
    {
        private SumOperatorTerm(Term higherOrder) =>
            this.HigherOrder = higherOrder;

        public override Term HigherOrder { get; }

        public override Term Infer(InferContext context)
        {
            var higherOrder = this.HigherOrder.Infer(context);

            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new SumOperatorTerm<TSumTerm>(higherOrder);
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
                new SumOperatorTerm<TSumTerm>(higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var higherOrder = this.HigherOrder.Fixup(context);

            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new SumOperatorTerm<TSumTerm>(higherOrder);
        }

        Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint)
        {
            var higherOrder = this.HigherOrder.Fixup(context);

            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new SumOperatorTerm<TSumTerm>(higherOrder);
        }

        public override Term Reduce(ReduceContext context)
        {
            var higherOrder = this.HigherOrder.Reduce(context);

            return object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                this :
                new SumOperatorTerm<TSumTerm>(higherOrder);
        }

        AppliedResult IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            AppliedResult.Applied(
                new SumClosureTerm(argument, this.HigherOrder.Reduce(context)),
                argument);

        public override bool Equals(Term? other) =>
            other is SumOperatorTerm<TSumTerm>;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            "+";

        public static readonly SumOperatorTerm<TSumTerm> Instance =
            new SumOperatorTerm<TSumTerm>(LambdaTerm.Unspecified3);

        public static Term Compose(IEnumerable<Term> terms) =>
            terms.Aggregate((Term)Instance, (agg, term) =>
                ApplyTerm.Create(agg, term, UnspecifiedTerm.Instance));

        private sealed class SumClosureTerm : Term, IApplicable
        {
            private readonly Term lhs;

            public SumClosureTerm(Term lhs, Term higherOrder)
            {
                this.lhs = lhs;
                this.HigherOrder = higherOrder;
            }

            public override Term HigherOrder { get; }

            public override Term Infer(InferContext context)
            {
                var higherOrder = this.HigherOrder.Infer(context);
                var lhs = this.lhs.Infer(context);

                context.Unify(lhs.HigherOrder, higherOrder);

                return
                    object.ReferenceEquals(lhs, this.lhs) &&
                    object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                        this :
                        new SumClosureTerm(lhs, higherOrder);
            }

            Term IApplicable.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint)
            {
                var higherOrder = this.HigherOrder.Infer(context);
                var lhs = this.lhs.Infer(context);

                // ? -> ?
                var higherOrderFromArgument = LambdaTerm.From(
                    lhs.HigherOrder,
                    inferredArgumentHint.HigherOrder);

                context.Unify(higherOrder, higherOrderHint);
                context.Unify(higherOrderFromArgument, higherOrderHint);

                return
                    object.ReferenceEquals(lhs, this.lhs) &&
                    object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                        this :
                        new SumClosureTerm(lhs, higherOrder);
            }

            public override Term Fixup(FixupContext context)
            {
                var higherOrder = this.HigherOrder.Fixup(context);
                var lhs = this.lhs.Fixup(context);

                return
                    object.ReferenceEquals(lhs, this.lhs) &&
                    object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                        this :
                        new SumClosureTerm(lhs, higherOrder);
            }

            Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint)
            {
                var higherOrder = this.HigherOrder.Fixup(context);
                var lhs = this.lhs.Fixup(context);

                return
                    object.ReferenceEquals(lhs, this.lhs) &&
                    object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                        this :
                        new SumClosureTerm(lhs, higherOrder);
            }

            public override Term Reduce(ReduceContext context)
            {
                var higherOrder = this.HigherOrder.Reduce(context);
                var lhs = this.lhs.Reduce(context);

                return
                    object.ReferenceEquals(lhs, this.lhs) &&
                    object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                        this :
                        new SumClosureTerm(lhs, higherOrder);
            }

            AppliedResult IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint)
            {
                var higherOrder = this.HigherOrder.Reduce(context);
                var lhs = this.lhs.Reduce(context);
                var rhs = argument.Reduce(context);

                // TODO:
                return AppliedResult.Ignored(null!, null!);
                //return AppliedResult.Applied(
                //    new TSumTerm(lhs, rhs, higherOrder), rhs);
            }

            public override bool Equals(Term? other) =>
                other is TSumTerm;

            protected override string OnPrettyPrint(PrettyPrintContext context) =>
                $"+ {this.lhs.PrettyPrint(context)}";

            public void Deconstruct(out Term lhs, out Term higherOrder)
            {
                lhs = this.lhs;
                higherOrder = this.HigherOrder;
            }
        }
    }
}
