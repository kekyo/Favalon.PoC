using Favalon.Terms.Algebraic;
using Favalon.Terms.Contexts;
using Favalon.Terms.Types;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon.Terms.Methods
{
    public sealed class ClrMethodOverloadedTerm : Term, IApplicableTerm
    {
        private static readonly int hashCode =
            typeof(ClrMethodOverloadedTerm).GetHashCode();

        public readonly ClrMethodTerm[] Methods;

        internal ClrMethodOverloadedTerm(ClrMethodTerm[] methods) =>
            this.Methods = methods;

        protected override Term GetHigherOrder() =>
            SumTerm.Composed(this.Methods.
                Select(method => method.HigherOrder).
                Distinct())!;

        public override Term Infer(InferContext context) =>
            // Best effort infer procedure: cannot fixed.
            this;

        Term IApplicableTerm.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint) =>
            ApplyTerm.AggregateForApply(this, this.Methods, inferredArgumentHint, higherOrderHint);

        public override Term Fixup(FixupContext context) =>
            // Best effort fixup procedure: cannot fixed.
            this;

        Term IApplicableTerm.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint) =>
            ApplyTerm.AggregateForApply(this, this.Methods, fixuppedArgumentHint, higherOrderHint);

        public override Term Reduce(ReduceContext context) =>
            this;

        Term? IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            ApplyTerm.ReduceForApply(this, this.Methods, context, argument, higherOrderHint);

        public override bool Equals(Term? other) =>
            other is ClrMethodOverloadedTerm rhs ?
                this.Methods.SequenceEqual(rhs.Methods) :
                false;

        public override int GetHashCode() =>
            this.Methods.Aggregate(hashCode, (v, method) => v ^ method.GetHashCode());

        protected override string OnPrettyPrint(PrettyPrintContext context)
        {
            var methods = Utilities.Join(
                " + ",
                this.Methods.Select(method => $"({method.PrettyPrint(context)})"));
            return $"({methods})";
        }
    }
}
