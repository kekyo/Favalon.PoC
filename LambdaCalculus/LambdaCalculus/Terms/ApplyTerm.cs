using Favalon.TermContexts;

namespace Favalon.Terms
{
    public struct AppliedResult
    {
        public readonly bool IsApplied;
        public readonly Term Result;
        public readonly Term Argument;

        private AppliedResult(bool isApplied, Term result, Term argument)
        {
            this.IsApplied = isApplied;
            this.Result = result;
            this.Argument = argument;
        }

        public void Deconstruct(out bool isApplied, out Term result, out Term argument)
        {
            isApplied = this.IsApplied;
            result = this.Result;
            argument = this.Argument;
        }

        public static AppliedResult Applied(Term applied, Term reducedArgument) =>
            new AppliedResult(true, applied, reducedArgument);

        public static AppliedResult Ignored(Term term, Term argument) =>
            new AppliedResult(false, term, argument);
    }

    // It's only using in ApplyTerm.
    public interface IApplicable
    {
        Term InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint);

        Term FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint);

        AppliedResult ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint);
    }

    public sealed class ApplyTerm : HigherOrderHoldTerm
    {
        public readonly Term Function;
        public readonly Term Argument;

        private ApplyTerm(Term function, Term argument, Term higherOrder) :
            base(higherOrder)
        {
            this.Function = function;
            this.Argument = argument;
        }

        //private static IEnumerable<T> GetFittedAndOrderedMethods<T>(T[] methods, Term parameterHigherOrder, Term returnHigherOrderHint)
        //    where T : Term =>
        //    methods.
        //        Select(method =>
        //            (method.HigherOrder is LambdaTerm methodHigherOrder) ?
        //                (method,
        //                 parameterType: TypeTerm.Narrow(methodHigherOrder.Parameter, parameterHigherOrder),
        //                 returnType: TypeTerm.Narrow(returnHigherOrderHint, methodHigherOrder.Body)) :
        //                (method, null, null)).
        //        Where(entry => entry.parameterType is Term && entry.returnType is Term).
        //        OrderBy(entry => entry.parameterType!, TypeTerm.ConcreterComparer).
        //        ThenBy(entry => entry.returnType!, TypeTerm.ConcreterComparer).
        //        Select(entry => entry.method);

        //internal static Term AggregateForApply(
        //    Term @this,
        //    Term[] terms,
        //    Term argument,
        //    Term higherOrderHint)
        //{
        //    // Strict infer procedure.

        //    var fittedMethods =
        //        GetFittedAndOrderedMethods(terms, argument.HigherOrder, higherOrderHint).
        //        ToArray();

        //    return fittedMethods.Length switch
        //    {
        //        // Matched single method:
        //        1 => fittedMethods[0],
        //        // No matched: it contains illegal terms, reducer cause error when will not fixed.
        //        0 => @this,
        //        // Matched multiple methods:
        //        _ => (fittedMethods.Length != terms.Length) ?
        //            // Partially matched.
        //            new SumTerm(fittedMethods) :
        //            // All matched: not changed.
        //            @this
        //    };
        //}

        public override Term Infer(InferContext context)
        {
            var argument = this.Argument.Infer(context);
            var higherOrder = context.ResolveHigherOrder(this);

            var function = this.Function switch
            {
                IApplicable applicable => applicable.InferForApply(context, argument, higherOrder),
                _ => this.Function.Infer(context)
            };

            // (f:('1 -> '2) a:'1):'2
            context.Unify(
                function.HigherOrder,
                LambdaTerm.From(argument.HigherOrder, higherOrder));

            return
                this.Function.EqualsWithHigherOrder(function) &&
                this.Argument.EqualsWithHigherOrder(argument) &&
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
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
                _ => this.Function.Fixup(context)
            };

            return
                this.Function.EqualsWithHigherOrder(function) &&
                this.Argument.EqualsWithHigherOrder(argument) &&
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    new ApplyTerm(function, argument, higherOrder);
        }

        public override Term Reduce(ReduceContext context)
        {
            var higherOrder = this.HigherOrder.Reduce(context);

            var function = this.Function;
            var argument = this.Argument;

            for (var iteration = 0; iteration < context.Iterations; iteration++)
            {
                if (function is IApplicable applicable)
                {
                    switch (applicable.ReduceForApply(context, argument, higherOrder))
                    {
                        case AppliedResult(true, Term reduced, _):
                            return reduced;
                        case AppliedResult(false, Term f, Term a):
                            function = f;
                            argument = a;
                            break;
                    }
                }
                else
                {
                    function = function.Reduce(context);
                }

                if (this.Function.EqualsWithHigherOrder(function) &&
                    this.Argument.EqualsWithHigherOrder(argument))
                {
                    break;
                }
            }

            // TODO: Detects uninterpretable terms on many iterations.

            return
                this.Function.EqualsWithHigherOrder(function) &&
                this.Argument.EqualsWithHigherOrder(argument) &&
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    new ApplyTerm(function, argument, higherOrder);
        }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is ApplyTerm rhs ?
                (this.Function.Equals(context, rhs.Function) && this.Argument.Equals(context, rhs.Argument)) :
                false;

        public override int GetHashCode() =>
            this.Function.GetHashCode() ^ this.Argument.GetHashCode();

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Function.PrettyPrint(context)} {this.Argument.PrettyPrint(context)}";

        public void Deconstruct(out Term function, out Term argument)
        {
            function = this.Function;
            argument = this.Argument;
        }

        public void Deconstruct(out Term function, out Term argument, out Term higherOrder)
        {
            function = this.Function;
            argument = this.Argument;
            higherOrder = this.HigherOrder;
        }

        public static ApplyTerm Create(Term function, Term argument, Term higherOrder) =>
            new ApplyTerm(function, argument, higherOrder);
    }
}
