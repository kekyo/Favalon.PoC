using Favalon.Contexts;
using Favalon.Terms.Logical;

namespace Favalon.Terms.Operators
{
    public sealed class IfOperatorTerm : OperatorSymbolTerm<IfOperatorTerm>, IApplicable
    {
        private static readonly Term higherOrder =
            LambdaTerm.Create(BooleanTerm.Type, LambdaTerm.Unspecified);

        private IfOperatorTerm()
        { }

        public override Term HigherOrder =>
            higherOrder;

        Term IApplicable.InferForApply(InferContext context, Term inferredArgument) =>
           this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term argument) =>
            new ConditionTerm(argument);   // NOT reduced at this time.

        public static readonly IfOperatorTerm Instance =
            new IfOperatorTerm();

        private sealed class ConditionTerm : OperatorArgument0Term<ConditionTerm>, IApplicable
        {
            private static readonly Term higherOrder =
                LambdaTerm.Create(UnspecifiedTerm.Instance, LambdaTerm.Unspecified);

            public ConditionTerm(Term condition) :
                base(condition)
            { }

            public override Term HigherOrder =>
                higherOrder;

            protected override Term Create(Term argument) =>
                new ConditionTerm(argument);

            Term IApplicable.InferForApply(InferContext context, Term inferredArgument) =>
               this;

            Term? IApplicable.ReduceForApply(ReduceContext context, Term argument) =>
                new ThenTerm(this.Argument0, argument);   // NOT reduced at this time.
        }

        private sealed class ThenTerm : OperatorArgument1Term<ThenTerm>, IApplicable
        {
            public ThenTerm(Term condition, Term then) :
                base(condition, then)
            { }

            public override Term HigherOrder =>
                LambdaTerm.Create(this.Argument1.HigherOrder, this.Argument1.HigherOrder);

            protected override Term Create(Term argument0, Term argument1) =>
                new ThenTerm(argument0, argument1);

            Term IApplicable.InferForApply(InferContext context, Term inferredArgument) =>
               this;

            Term? IApplicable.ReduceForApply(ReduceContext context, Term argument) =>
                IfTerm.Reduce(context, this.Argument0, this.Argument1, argument);
        }
    }
}
