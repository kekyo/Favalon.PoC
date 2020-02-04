using Favalon.Terms.Contexts;
using Favalon.Terms.Types;
using System.Linq;

namespace Favalon.Terms
{
    public sealed class LambdaTerm : HigherOrderLazyTerm, IApplicableTerm, IRightToLeftPrettyPrintingTerm
    {
        public readonly Term Parameter;
        public readonly Term Body;

        private LambdaTerm(Term parameter, Term body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        internal override bool ValidTerm =>
            this.Parameter.ValidTerm && this.Body.ValidTerm;

        protected override Term GetHigherOrder() =>
            From(this.Parameter.HigherOrder, this.Body.HigherOrder);

        public override Term Infer(InferContext context)
        {
            // Best effort infer procedure.

            var newScope = context.NewScope();

            var parameter = this.Parameter.Infer(newScope);
            if (parameter is FreeVariableTerm identity)
            {
                // Shadowed just parameter, will transfer parameter higherorder.
                newScope.BindMutable(identity.Identity, parameter);
            }

            // Calculate inferring with parameter identity.
            var body = this.Body.Infer(newScope);

            return
                this.Parameter.EqualsWithHigherOrder(parameter) &&
                this.Body.EqualsWithHigherOrder(body) ?
                    this :
                    From(parameter, body);
        }

        Term IApplicableTerm.InferForApply(InferContext context, Term inferredArgumentHint, Term appliedHigherOrderHint)
        {
            // Strict infer procedure.

            var newScope = context.NewScope();

            var parameter = this.Parameter.Infer(newScope);
            if (parameter is FreeVariableTerm identity)
            {
                // Applied argument.
                newScope.BindMutable(identity.Identity, inferredArgumentHint);
            }

            // Calculate inferring with applied argument.
            var body = this.Body.Infer(newScope);

            context.Unify(body.HigherOrder, appliedHigherOrderHint);

            return
                this.Parameter.EqualsWithHigherOrder(parameter) &&
                this.Body.EqualsWithHigherOrder(body) ?
                    this :
                    From(parameter, body);
        }

        public override Term Fixup(FixupContext context)
        {
            // Best effort fixup procedure.

            var parameter = this.Parameter.Fixup(context);
            var body = this.Body.Fixup(context);

            return
                this.Parameter.EqualsWithHigherOrder(parameter) &&
                this.Body.EqualsWithHigherOrder(body) ?
                    this :
                    From(parameter, body);
        }

        Term IApplicableTerm.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term appliedHigherOrderHint)
        {
            // Strict fixup procedure.

            var parameter = this.Parameter.Fixup(context);
            var body = this.Body.Fixup(context);

            return
                this.Parameter.EqualsWithHigherOrder(parameter) &&
                this.Body.EqualsWithHigherOrder(body) ?
                    this :
                    From(parameter, body);
        }

        public override Term Reduce(ReduceContext context)
        {
            var newScope = context.NewScope();

            var parameter = this.Parameter.Reduce(context);
            if (parameter is FreeVariableTerm identity)
            {
                // Shadowed just parameter, will transfer parameter higherorder.
                newScope.BindMutable(identity.Identity, identity);
            }

            var body = this.Body.Reduce(newScope);

            return
                this.Parameter.EqualsWithHigherOrder(parameter) &&
                this.Body.EqualsWithHigherOrder(body) ?
                    this :
                    From(parameter, body);
        }

        AppliedResult IApplicableTerm.ReduceForApply(ReduceContext context, Term argument, Term appliedHigherOrderHint)
        {
            // The parameter and argument are out of inner scope.
            var parameter = this.Parameter.Reduce(context);

            if (parameter is FreeVariableTerm identity)
            {
                var reducedArgument = argument.Reduce(context);

                // Bound on inner scope
                var newScope = context.NewScope();
                newScope.BindMutable(identity.Identity, reducedArgument);

                var reducedBody = this.Body.Reduce(newScope);

                return AppliedResult.Applied(reducedBody, reducedArgument);
            }
            // Cannot get identity (cannot apply)
            else
            {
                // Cannot reduce the body, so recreate lambda with reduced parameter.
                return AppliedResult.Ignored(
                    From(parameter, this.Body),
                    argument);
            }
        }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is LambdaTerm rhs ?
                (this.Parameter.Equals(context, rhs.Parameter) && this.Body.Equals(context, rhs.Body)) :
                false;

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^ this.Body.GetHashCode();

        public void Deconstruct(out Term parameter, out Term body)
        {
            parameter = this.Parameter;
            body = this.Body;
        }

        public void Deconstruct(out Term parameter, out Term body, out Term higherOrder)
        {
            parameter = this.Parameter;
            body = this.Body;
            higherOrder = this.HigherOrder;
        }

        protected override bool IsIncludeHigherOrderInPrettyPrinting(HigherOrderDetails higherOrderDetail) =>
            false;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Parameter.PrettyPrint(context)} -> {this.Body.PrettyPrint(context)}";

        public static Term From(Term parameter, Term body) =>
            (parameter, body) switch
            {
                (null, null) => TerminationTerm.Instance,
                (TerminationTerm _, TerminationTerm _) => TerminationTerm.Instance,
                (Term _, null) => From(parameter, UnspecifiedTerm.Instance),
                (null, Term _) => From(UnspecifiedTerm.Instance, body),
                (UnspecifiedTerm _, UnspecifiedTerm _) => Unspecified,
                (KindTerm _, KindTerm _) when
                    parameter.Equals(KindTerm.Instance) && body.Equals(KindTerm.Instance) => Kind,
                _ => new LambdaTerm(parameter, body)
            };

        public static Term Repeat(Term term, int termCount) =>
            Enumerable.
                Range(0, termCount).
                Aggregate(term, (agg, _) => From(term, agg));

        // _ -> _
        public static readonly LambdaTerm Unspecified =
            new LambdaTerm(UnspecifiedTerm.Instance, UnspecifiedTerm.Instance);

        // _ -> _ -> _
        public static readonly LambdaTerm Unspecified2 =
            new LambdaTerm(UnspecifiedTerm.Instance, Unspecified);

        // * -> *
        public static readonly LambdaTerm Kind =
            new LambdaTerm(KindTerm.Instance, KindTerm.Instance);

        // * -> * -> *
        public static readonly LambdaTerm Kind2 =
            new LambdaTerm(KindTerm.Instance, Kind);
    }
}
