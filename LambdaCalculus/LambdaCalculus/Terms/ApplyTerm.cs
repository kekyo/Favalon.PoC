using Favalon.Contexts;
using System.Diagnostics;

namespace Favalon.Terms
{
    // It's only using in ApplyTerm.
    public interface IApplicable
    {
        Term InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint);

        Term FixupForApply(FixupContext context, Term fixuppedArgument, Term higherOrderHint);

        Term? ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint);
    }

    public sealed class ApplyTerm : HigherOrderHoldTerm
    {
        public readonly Term Function;
        public readonly Term Argument;

        internal ApplyTerm(Term function, Term argument, Term higherOrder) :
            base(higherOrder)
        {
            this.Function = function;
            this.Argument = argument;
        }

        private static IEnumerable<T> GetFittedAndOrderedMethods<T>(T[] methods, Term parameterHigherOrder, Term returnHigherOrderHint)
            where T : Term =>
            methods.
                Select(method =>
                    (method.HigherOrder is LambdaTerm methodHigherOrder) ?
                        (method,
                         parameterType: TypeTerm.Narrow(methodHigherOrder.Parameter, parameterHigherOrder),
                         returnType: TypeTerm.Narrow(returnHigherOrderHint, methodHigherOrder.Body)) :
                        (method, null, null)).
                Where(entry => entry.parameterType is Term && entry.returnType is Term).
                OrderBy(entry => entry.parameterType!, TypeTerm.ConcreterComparer).
                ThenBy(entry => entry.returnType!, TypeTerm.ConcreterComparer).
                Select(entry => entry.method);

        private static Term InferForApply(
            Term @this,
            Term[] methods,
            InferContext context,
            Term inferredArgument,
            Term higherOrderHint)
        {
            // Strict infer procedure.

            var fittedMethods =
                GetFittedAndOrderedMethods(methods, inferredArgument.HigherOrder, higherOrderHint).
                ToArray();

            return fittedMethods.Length switch
            {
                // Matched single method:
                1 => fittedMethods[0],
                // No matched: it contains illegal terms, reducer cause error when will not fixed.
                0 => @this,
                // Matched multiple methods:
                _ => (fittedMethods.Length != methods.Length) ?
                    // Partially matched.
                    new SumTerm(fittedMethods) :
                    // All matched: not changed.
                    @this
            };
        }

        public override Term Infer(InferContext context)
        {
            var argument = this.Argument.Infer(context);
            var higherOrder = this.HigherOrder.Infer(context);

            var function = (this.Function is IApplicable applicable) ?
                applicable.InferForApply(context, argument, higherOrder) :
                this.Function.Infer(context);

            // (f:('1 -> '2) a:'1):'2
            context.Unify(
                function.HigherOrder,
                LambdaTerm.Create(argument.HigherOrder, higherOrder));

            return
                object.ReferenceEquals(function, this.Function) &&
                object.ReferenceEquals(argument, this.Argument) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                    this :
                    new ApplyTerm(function, argument, higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var argument = this.Argument.Fixup(context);
            var higherOrder = this.HigherOrder.Fixup(context);

            var function = (this.Function is IApplicable applicable) ?
                applicable.FixupForApply(context, argument, higherOrder) :
                this.Function.Fixup(context);

            return
                object.ReferenceEquals(function, this.Function) &&
                object.ReferenceEquals(argument, this.Argument) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                    this :
                    new ApplyTerm(function, argument, higherOrder);
        }

        public override Term Reduce(ReduceContext context)
        {
            var function = this.Function.Reduce(context);
            var higherOrder = this.HigherOrder.Reduce(context);

            if (function is IApplicable applicable &&
                applicable.ReduceForApply(context, this.Argument, higherOrder) is Term term)
            {
                // TODO:
                //Debug.Assert(term.HigherOrder.Equals(higherOrder));

                return term;
            }

            var argument = this.Argument.Reduce(context);   // TODO: Reduced twice

            return
                object.ReferenceEquals(function, this.Function) &&
                object.ReferenceEquals(argument, this.Argument) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                    this :
                    new ApplyTerm(function, argument, higherOrder);
        }

        public override bool Equals(Term? other) =>
            other is ApplyTerm rhs ?
                (this.Function.Equals(rhs.Function) && this.Argument.Equals(rhs.Argument)) :
                false;

        public override int GetHashCode() =>
            this.Function.GetHashCode() ^ this.Argument.GetHashCode();
    }
}
