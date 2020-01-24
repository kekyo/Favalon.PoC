using Favalon.Terms.Algebraic;
using Favalon.Terms.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon.Terms.Types
{
    public sealed class ClrMethodTerm : Term, IValueTerm, IApplicableTerm
    {
        private readonly MethodInfo method;

        internal ClrMethodTerm(MethodInfo method) =>
            this.method = method;

        public override Term HigherOrder =>
            GetMethodHigherOrder(this.method);

        object IValueTerm.Value =>
            this.method;

        public override Term Infer(InferContext context) =>
            this;

        Term IApplicableTerm.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint)
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

        Term IApplicableTerm.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        internal Term Invoke(IValueTerm argument) =>
            ClrConstantTerm.From(this.method.Invoke(null, new object[] { argument.Value }));

        AppliedResult IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            argument.Reduce(context) is Term argumentTerm ?
                (argumentTerm is IValueTerm valueTerm ?
                    AppliedResult.Applied(this.Invoke(valueTerm), argumentTerm) :
                    AppliedResult.Ignored(this, argumentTerm)) :
                AppliedResult.Ignored(this, argument);

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is ClrMethodTerm rhs ?
                this.method.Equals(rhs.method) :
                false;

        public override int GetHashCode() =>
            this.method.GetHashCode();

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.method.DeclaringType.PrettyPrint(context)}.{this.method.Name}";

        private static LambdaTerm GetMethodHigherOrder(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return LambdaTerm.From(
                ClrTypeTerm.From((parameters.Length == 0) ? typeof(void) : parameters[0].ParameterType),
                ClrTypeTerm.From(method.ReturnType));
        }

        public static Term From(IEnumerable<MethodInfo> methods)
        {
            var ms = methods.ToArray();
            return (ms.Length == 1) ?
                (Term)new ClrMethodTerm(ms[0]) :
                null!;
                //new ClrMethodOverloadedTerm(ms.Select(method => new ClrMethodTerm(method)).ToArray());
        }
    }

    //public sealed class ClrMethodOverloadedTerm : Term, IApplicableTerm
    //{
    //    private static readonly int hashCode =
    //        typeof(ClrMethodOverloadedTerm).GetHashCode();

    //    public readonly ClrMethodTerm[] Methods;

    //    internal ClrMethodOverloadedTerm(ClrMethodTerm[] methods) =>
    //        this.Methods = methods;

    //    protected override Term GetHigherOrder() =>
    //        SumTerm.Composed(this.Methods.
    //            Select(method => method.HigherOrder).
    //            Distinct())!;

    //    public override Term Infer(InferContext context) =>
    //        // Best effort infer procedure: cannot fixed.
    //        this;

    //    Term IApplicableTerm.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint) =>
    //        ApplyTerm.AggregateForApply(this, this.Methods, inferredArgumentHint, higherOrderHint);

    //    public override Term Fixup(FixupContext context) =>
    //        // Best effort fixup procedure: cannot fixed.
    //        this;

    //    Term IApplicableTerm.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint) =>
    //        ApplyTerm.AggregateForApply(this, this.Methods, fixuppedArgumentHint, higherOrderHint);

    //    public override Term Reduce(ReduceContext context) =>
    //        this;

    //    Term? IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
    //        ApplyTerm.ReduceForApply(this, this.Methods, context, argument, higherOrderHint);

    //    public override bool Equals(Term? other) =>
    //        other is ClrMethodOverloadedTerm rhs ?
    //            this.Methods.SequenceEqual(rhs.Methods) :
    //            false;

    //    public override int GetHashCode() =>
    //        this.Methods.Aggregate(hashCode, (v, method) => v ^ method.GetHashCode());

    //    protected override string OnPrettyPrint(PrettyPrintContext context)
    //    {
    //        var methods = Utilities.Join(
    //            " + ",
    //            this.Methods.Select(method => $"({method.PrettyPrint(context)})"));
    //        return $"({methods})";
    //    }
    //}
}
