using Favalon.Contexts;
using Favalon.Terms.Algebric;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon.Terms.Types
{
    public abstract class MethodTerm : HigherOrderLazyTerm
    {
        private protected MethodTerm()
        { }

        private protected static LambdaTerm GetMethodHigherOrder(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return LambdaTerm.Create(
                TypeTerm.From((parameters.Length == 0) ? typeof(void) : parameters[0].ParameterType),
                TypeTerm.From(method.ReturnType));
        }

        public static MethodTerm From(IEnumerable<MethodInfo> methods)
        {
            var ms = methods.ToArray();
            return (ms.Length == 1) ?
                (MethodTerm)new ClrMethodTerm(ms[0]) :
                new ClrMethodOverloadedTerm(ms.Select(method => new ClrMethodTerm(method)).ToArray());
        }
    }

    public sealed class ClrMethodTerm : MethodTerm, IApplicable
    {
        private readonly MethodInfo method;

        internal ClrMethodTerm(MethodInfo method) =>
            this.method = method;

        protected override Term GetHigherOrder() =>
            GetMethodHigherOrder(this.method);

        public Term ParameterHigherOrder =>
            ((LambdaTerm)this.HigherOrder).Parameter;

        public Term ReturnHigherOrder =>
            ((LambdaTerm)this.HigherOrder).Body;

        public override Term Infer(InferContext context) =>
            this;

        Term IApplicable.InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint)
        {
            context.Unify(
                ((LambdaTerm)this.HigherOrder).Parameter,
                inferredArgument.HigherOrder);
            context.Unify(
                ((LambdaTerm)this.HigherOrder).Body,
                higherOrderHint);

            return this;
        }

        public override Term Fixup(FixupContext context) =>
            this;

        Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgument, Term higherOrderHint) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        internal ConstantTerm Invoke(ConstantTerm constantArgument) =>
            new ConstantTerm(this.method.Invoke(null, new object[] { constantArgument.Value }));

        Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            (argument.Reduce(context) is ConstantTerm constantArgument) ?
                this.Invoke(constantArgument) :
                null;

        public override bool Equals(Term? other) =>
            other is ClrMethodTerm rhs ?
                this.method.Equals(rhs.method) :
                false;

        public override int GetHashCode() =>
            this.method.GetHashCode();
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

        private IEnumerable<ClrMethodTerm> GetFittedAndOrderedMethods(Term parameterHigherOrder, Term returnHigherOrderHint) =>
            this.Methods.
                Select(method => (
                    method,
                    parameterType: TypeTerm.Narrow(method.ParameterHigherOrder, parameterHigherOrder),
                    returnType: TypeTerm.Narrow(returnHigherOrderHint, method.ReturnHigherOrder))).
                Where(entry => entry.parameterType is Term && entry.returnType is Term).
                OrderBy(entry => entry.parameterType!, TypeTerm.ConcreterComparer).
                ThenBy(entry => entry.returnType!, TypeTerm.ConcreterComparer).
                Select(entry => entry.method);

        public override Term Infer(InferContext context) =>
            // Best effort infer procedure: cannot fixed.
            this;

        Term IApplicable.InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint)
        {
            // Strict infer procedure.

            var fittedMethods =
                this.GetFittedAndOrderedMethods(inferredArgument.HigherOrder, higherOrderHint).
                ToArray();

            return fittedMethods.Length switch
            {
                // Matched single method:
                1 => fittedMethods[0],
                // No matched: it contains illegal terms, reducer cause error when will not fixed.
                0 => this,
                // Matched multiple methods:
                _ => (fittedMethods.Length != this.Methods.Length) ?
                    // Partially matched.
                    new ClrMethodOverloadedTerm(fittedMethods) :
                    // All matched: not changed.
                    this
            };
        }

        public override Term Fixup(FixupContext context) =>
            // Best effort fixup procedure: cannot fixed.
            this;

        Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgument, Term higherOrderHint)
        {
            // Strict fixup procedure.

            var fittedMethods =
                this.GetFittedAndOrderedMethods(fixuppedArgument.HigherOrder, higherOrderHint).
                ToArray();

            return fittedMethods.Length switch
            {
                // Matched single method:
                1 => fittedMethods[0],
                // No matched: it contains illegal terms, reducer cause error when will not fixed.
                0 => this,
                // Matched multiple methods:
                _ => (fittedMethods.Length != this.Methods.Length) ?
                    // Partially matched.
                    new ClrMethodOverloadedTerm(fittedMethods) :
                    // All matched: not changed.
                    this
            };
        }

        public override Term Reduce(ReduceContext context) =>
            this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            (argument.Reduce(context) is ConstantTerm constantArgument) ?
                this.GetFittedAndOrderedMethods(constantArgument.HigherOrder, higherOrderHint).
                FirstOrDefault()?.Invoke(constantArgument) :
                null;

        public override bool Equals(Term? other) =>
            other is ClrMethodOverloadedTerm rhs ?
                this.Methods.SequenceEqual(rhs.Methods) :
                false;

        public override int GetHashCode() =>
            this.Methods.Aggregate(hashCode, (v, method) => v ^ method.GetHashCode());
    }
}
