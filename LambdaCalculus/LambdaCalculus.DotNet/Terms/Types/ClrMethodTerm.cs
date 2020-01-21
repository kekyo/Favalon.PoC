using Favalon.Contexts;
using Favalon.Terms.Algebric;
using LambdaCalculus.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon.Terms.Types
{
    public sealed class ClrMethodTerm : MethodTerm, IApplicable
    {
        private readonly MethodInfo method;

        internal ClrMethodTerm(MethodInfo method) =>
            this.method = method;

        protected override Term GetHigherOrder() =>
            GetMethodHigherOrder(this.method);

        public override Term Infer(InferContext context) =>
            this;

        Term IApplicable.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint)
        {
            context.Unify(
                ((LambdaTerm)this.HigherOrder).Parameter,
                inferredArgumentHint.HigherOrder);
            context.Unify(
                ((LambdaTerm)this.HigherOrder).Body,
                higherOrderHint);

            return this;
        }

        public override Term Fixup(FixupContext context) =>
            this;

        Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        internal ConstantTerm Invoke(ConstantTerm constantArgument) =>
            new ConstantTerm(this.method.Invoke(null, new object[] { constantArgument.Value }));

        AppliedResult IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            (argument.Reduce(context) is ConstantTerm constantArgument) ?
                AppliedResult.Applied(this.Invoke(constantArgument), constantArgument) :
                AppliedResult.Ignored(this, argument);

        public override bool Equals(Term? other) =>
            other is ClrMethodTerm rhs ?
                this.method.Equals(rhs.method) :
                false;

        public override int GetHashCode() =>
            this.method.GetHashCode();

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.method.DeclaringType.PrettyPrint(context)}.{this.method.Name}";
    }

    public sealed class ClrMethodOverloadedTerm : MethodTerm, IApplicable
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

        Term IApplicable.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint) =>
            ApplyTerm.AggregateForApply(this, this.Methods, inferredArgumentHint, higherOrderHint);

        public override Term Fixup(FixupContext context) =>
            // Best effort fixup procedure: cannot fixed.
            this;

        Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint) =>
            ApplyTerm.AggregateForApply(this, this.Methods, fixuppedArgumentHint, higherOrderHint);

        public override Term Reduce(ReduceContext context) =>
            this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
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
