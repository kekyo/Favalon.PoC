using Favalon.Contexts;

namespace Favalon.Terms
{
    public sealed class LambdaTerm : HigherOrderLazyTerm, IApplicable, IRightToLeftPrettyPrintingTerm
    {
        public readonly Term Parameter;
        public readonly Term Body;

        private LambdaTerm(Term parameter, Term body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

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
                newScope.SetBoundTerm(identity.Identity, parameter);
            }

            // Calculate inferring with parameter identity.
            var body = this.Body.Infer(newScope);

            return
                this.Parameter.EqualsWithHigherOrder(parameter) &&
                this.Body.EqualsWithHigherOrder(body) ?
                    this :
                    From(parameter, body);
        }

        Term IApplicable.InferForApply(InferContext context, Term inferredArgumentHint, Term higherOrderHint)
        {
            // Strict infer procedure.

            var newScope = context.NewScope();

            var parameter = this.Parameter.Infer(newScope);
            if (parameter is FreeVariableTerm identity)
            {
                // Applied argument.
                newScope.SetBoundTerm(identity.Identity, inferredArgumentHint);
            }

            // Calculate inferring with applied argument.
            var body = this.Body.Infer(newScope);

            context.Unify(body.HigherOrder, higherOrderHint);

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

        Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgumentHint, Term higherOrderHint)
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
                newScope.SetBoundTerm(identity.Identity, identity);
            }

            var body = this.Body.Reduce(newScope);

            return
                this.Parameter.EqualsWithHigherOrder(parameter) &&
                this.Body.EqualsWithHigherOrder(body) ?
                    this :
                    From(parameter, body);
        }

        AppliedResult IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint)
        {
            // The parameter and argument are out of inner scope.
            var parameter = this.Parameter.Reduce(context);

            if (parameter is FreeVariableTerm identity)
            {
                var reducedArgument = argument.Reduce(context);

                // Bound on inner scope
                var newScope = context.NewScope();
                newScope.SetBoundTerm(identity.Identity, reducedArgument);

                var reducedBody = this.Body.Reduce(newScope);

                return AppliedResult.Applied(reducedBody, reducedArgument);
            }
            // Cannot get identity (cannot apply)
            else
            {
                // Cannot reduce the body, so recreate lambda with reduced parameter.
                return AppliedResult.Ignored(
                    LambdaTerm.From(parameter, this.Body),
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

        protected override bool IsInclude(HigherOrderDetails higherOrderDetail) =>
            base.IsInclude(higherOrderDetail) &&
            this.Parameter.HigherOrder is Term &&
            this.Body.HigherOrder is Term;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Parameter.PrettyPrint(context)} -> {this.Body.PrettyPrint(context)}";

        public static LambdaTerm From(Term parameter, Term body) =>
            (parameter, body) switch
            {
                (Term _, null) => From(parameter, UnspecifiedTerm.Instance),
                (null, Term _) => From(UnspecifiedTerm.Instance, body),
                (UnspecifiedTerm _, UnspecifiedTerm _) => Unspecified,
                (KindTerm _, KindTerm _) => Kind,
                (LambdaTerm(Term p, Term b), _) => new LambdaTerm(From(p, b), body),
                (_, LambdaTerm(Term p, Term b)) => new LambdaTerm(parameter, From(p, b)),
                _ => new LambdaTerm(parameter, body)
            };

        // ? -> ?
        public static readonly LambdaTerm Unspecified =
            new LambdaTerm(UnspecifiedTerm.Instance, UnspecifiedTerm.Instance);

        // ? -> ? -> ?
        public static readonly LambdaTerm Unspecified3 =
            new LambdaTerm(UnspecifiedTerm.Instance, Unspecified);

        // * -> *
        public static readonly LambdaTerm Kind =
            new LambdaTerm(KindTerm.Instance, KindTerm.Instance);
    }
}
