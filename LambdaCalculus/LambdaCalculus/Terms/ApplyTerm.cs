using Favalon.Contexts;
using Favalon.Terms.Algebric;
using Favalon.Terms.Types;
using LambdaCalculus.Contexts;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        internal static Term AggregateForApply(
            Term @this,
            Term[] terms,
            Term argument,
            Term higherOrderHint)
        {
            // Strict infer procedure.

            var fittedMethods =
                GetFittedAndOrderedMethods(terms, argument.HigherOrder, higherOrderHint).
                ToArray();

            return fittedMethods.Length switch
            {
                // Matched single method:
                1 => fittedMethods[0],
                // No matched: it contains illegal terms, reducer cause error when will not fixed.
                0 => @this,
                // Matched multiple methods:
                _ => (fittedMethods.Length != terms.Length) ?
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

            var function = this.Function switch
            {
                IApplicable applicable => applicable.InferForApply(context, argument, higherOrder),
                SumTerm sum => AggregateForApply(sum, sum.Terms, argument, higherOrder),
                _ => this.Function.Infer(context)
            };

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

            var function = this.Function switch
            {
                IApplicable applicable => applicable.FixupForApply(context, argument, higherOrder),
                SumTerm sum => AggregateForApply(sum, sum.Terms, argument, higherOrder),
                _ => this.Function.Fixup(context)
            };

            return
                object.ReferenceEquals(function, this.Function) &&
                object.ReferenceEquals(argument, this.Argument) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                    this :
                    new ApplyTerm(function, argument, higherOrder);
        }

        internal static Term? ReduceForApply(
            Term @this,
            Term[] terms,
            ReduceContext context,
            Term argument,
            Term higherOrderHint)

        {
            var applicables =
                GetFittedAndOrderedMethods(terms, argument.HigherOrder, higherOrderHint).
                ToArray();

            return applicables.FirstOrDefault() switch
            {
                // Matched single method:
                IApplicable applicable => applicable.ReduceForApply(context, argument, higherOrderHint),
                // Matched multiple methods:
                _ => (applicables.Length != terms.Length) ?
                    // Partially matched.
                    new SumTerm(applicables) :
                    // All matched: not changed.
                    @this
            };
        }

        public override Term Reduce(ReduceContext context)
        {
            var higherOrder = this.HigherOrder.Reduce(context);

            Term? function = this.Function;
            while (true)
            {
                if (function is IApplicable applicable)
                {
                    function = applicable.ReduceForApply(context, this.Argument, higherOrder);
                    if (function is Term)
                    {
                        return function;
                    }
                }
                else if (function is SumTerm sum)
                {
                    function = ReduceForApply(sum, sum.Terms, context, this.Argument, higherOrder);
                    if (function is Term)
                    {
                        return function;
                    }
                }
                else
                {
                    function = function!.Reduce(context);
                    break;
                }
            }

            var argument = this.Argument.Reduce(context);

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

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Function.PrettyPrint(context)} {this.Argument.PrettyPrint(context)}";
    }
}
