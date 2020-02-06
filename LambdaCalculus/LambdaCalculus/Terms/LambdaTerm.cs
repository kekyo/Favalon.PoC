using Favalon.Terms.Contexts;
using Favalon.Terms.Types;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Terms
{
    public sealed class LambdaTerm : Term, IApplicableTerm, IRightToLeftPrettyPrintingTerm
    {
        public readonly Term Parameter;
        public readonly Term Body;

        private Term? higherOrder;

        private LambdaTerm(Term parameter, Term body, Term? higherOrder)
        {
            this.Parameter = parameter;
            this.Body = body;
            this.higherOrder = higherOrder;
        }

        public override Term HigherOrder
        {
            get
            {
                if (higherOrder == null)
                {
                    higherOrder = From(Parameter.HigherOrder, Body.HigherOrder);
                }

                return higherOrder;
            }
        }

        internal override bool ValidTerm =>
            this.Parameter.ValidTerm && this.Body.ValidTerm;

        public override Term Infer(InferContext context)
        {
            var newScope = context.NewScope();

            var parameter = this.Parameter.Infer(newScope);
            if (parameter is BoundIdentityTerm identity)
            {
                // Shadowed just parameter, will transfer parameter higher order.
                newScope.BindMutable(
                    identity.Identity,
                    FreeVariableTerm.Create(identity.Identity, identity.HigherOrder));
            }

            // Calculate inferring with parameter identity.
            var body = this.Body.Infer(newScope);

            return
                this.Parameter.EqualsWithHigherOrder(parameter) &&
                this.Body.EqualsWithHigherOrder(body) ?
                    this :
                    From(parameter, body);
        }

        public override Term Fixup(FixupContext context)
        {
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
            if (parameter is BoundIdentityTerm identity)
            {
                // Shadowed just parameter, will transfer parameter higher order.
                newScope.BindMutable(
                    identity.Identity,
                    FreeVariableTerm.Create(identity.Identity, identity.HigherOrder));
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
            if (parameter is BoundIdentityTerm identity)
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

        private static Term From(Term parameter, Term body, Term? higherOrder) =>
            (parameter, body) switch
            {
                (null, null) => TerminationTerm.Instance,
                (TerminationTerm _, TerminationTerm _) => TerminationTerm.Instance,
                (Term _, null) => From(parameter, UnspecifiedTerm.Instance, higherOrder),
                (null, Term _) => From(UnspecifiedTerm.Instance, body, higherOrder),
                (UnspecifiedTerm _, UnspecifiedTerm _) => Unspecified,
                (KindTerm _, KindTerm _) when
                    parameter.Equals(KindTerm.Instance) && body.Equals(KindTerm.Instance) => Kind,
                _ => new LambdaTerm(parameter, body, higherOrder)
            };

        public static Term From(Term parameter, Term body) =>
            From(parameter, body, null);

        public static Term? From(IEnumerable<Term> terms) =>
            terms.Memoize() switch
            {
                (_, 0) => null,
                (Term[] ts, 1) => ts[0],
                (Term[] ts, _) => ts.Reverse().Aggregate(ts.Last(), (agg, term) => From(term, agg))
            };

        public static Term? Repeat(Term term, int termCount) =>
            From(Enumerable.Repeat(term, termCount).Memoize());

        // _ -> _
        public static readonly LambdaTerm Unspecified =
            new LambdaTerm(UnspecifiedTerm.Instance, UnspecifiedTerm.Instance, null!);

        // _ -> _ -> _
        public static readonly LambdaTerm Unspecified2 =
            new LambdaTerm(UnspecifiedTerm.Instance, Unspecified, null!);

        // * -> *
        public static readonly LambdaTerm Kind =
            new LambdaTerm(KindTerm.Instance, KindTerm.Instance, null!);

        // * -> * -> *
        public static readonly LambdaTerm Kind2 =
            new LambdaTerm(KindTerm.Instance, Kind, null!);
    }
}
