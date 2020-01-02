using Favalon.Contexts;
using Favalon.Terms.Types;
using LambdaCalculus.Contexts;
using System.Collections.Generic;

namespace Favalon.Terms
{
    public sealed class LambdaTerm : HigherOrderLazyTerm, IApplicable
    {
        public readonly Term Parameter;
        public readonly Term Body;

        private LambdaTerm(Term parameter, Term body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        protected override Term GetHigherOrder() =>
            Create(this.Parameter.HigherOrder, this.Body.HigherOrder);

        public override Term Infer(InferContext context)
        {
            // Best effort infer procedure.

            var newScope = context.NewScope();

            var parameter = this.Parameter.Infer(newScope);
            if (parameter is IdentityTerm identity)
            {
                // Shadowed just parameter, will transfer parameter higherorder.
                newScope.SetBoundTerm(identity.Identity, parameter);
            }

            // Calculate inferring with parameter identity.
            var body = this.Body.Infer(newScope);

            return
                object.ReferenceEquals(parameter, this.Parameter) &&
                object.ReferenceEquals(body, this.Body) ?
                    this :
                    Create(parameter, body);
        }

        Term IApplicable.InferForApply(InferContext context, Term inferredArgument, Term higherOrderHint)
        {
            // Strict infer procedure.

            var newScope = context.NewScope();

            var parameter = this.Parameter.Infer(newScope);
            if (parameter is IdentityTerm identity)
            {
                // Applied argument.
                newScope.SetBoundTerm(identity.Identity, inferredArgument);
            }

            // Calculate inferring with applied argument.
            var body = this.Body.Infer(newScope);

            context.Unify(body.HigherOrder, higherOrderHint);

            return
                object.ReferenceEquals(parameter, this.Parameter) &&
                object.ReferenceEquals(body, this.Body) ?
                    this :
                    Create(parameter, body);
        }

        public override Term Fixup(FixupContext context)
        {
            // Best effort fixup procedure.

            var parameter = this.Parameter.Fixup(context);
            var body = this.Body.Fixup(context);

            return
                object.ReferenceEquals(parameter, this.Parameter) &&
                object.ReferenceEquals(body, this.Body) ?
                    this :
                    Create(parameter, body);
        }

        Term IApplicable.FixupForApply(FixupContext context, Term fixuppedArgument, Term higherOrderHint)
        {
            // Strict fixup procedure.

            var parameter = this.Parameter.Fixup(context);
            var body = this.Body.Fixup(context);

            return
                object.ReferenceEquals(parameter, this.Parameter) &&
                object.ReferenceEquals(body, this.Body) ?
                    this :
                    Create(parameter, body);
        }

        public override Term Reduce(ReduceContext context)
        {
            var newScope = context.NewScope();

            var parameter = this.Parameter.Reduce(context);
            if (parameter is IdentityTerm identity)
            {
                // Shadowed just parameter, will transfer parameter higherorder.
                newScope.SetBoundTerm(identity.Identity, identity);
            }

            var body = this.Body.Reduce(context);

            return
                object.ReferenceEquals(parameter, this.Parameter) &&
                object.ReferenceEquals(body, this.Body) ?
                    this :
                    Create(parameter, body);
        }

        Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint)
        {
            var newScope = context.NewScope();

            // It'll maybe make identity because already reduced by previous called Reduce().
            if (this.Parameter is IdentityTerm identity)
            {
                var argument_ = argument.Reduce(context);
                newScope.SetBoundTerm(identity.Identity, argument_);

                return this.Body.Reduce(newScope);
            }
            // Cannot get identity (cannot apply)
            else
            {
                return null;
            }
        }

        public override bool Equals(Term? other) =>
            other is LambdaTerm rhs ?
                (this.Parameter.Equals(rhs.Parameter) && this.Body.Equals(rhs.Body)) :
                false;

        public override int GetHashCode() =>
            this.Parameter.GetHashCode() ^ this.Body.GetHashCode();

        public void Deconstruct(out Term parameter, out Term body)
        {
            parameter = this.Parameter;
            body = this.Body;
        }

        protected override bool IsInclude(HigherOrderDetails higherOrderDetail) =>
            base.IsInclude(higherOrderDetail) &&
            this.Parameter.HigherOrder is Term &&
            this.Body.HigherOrder is Term;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Parameter.PrettyPrint(context)} -> {this.Body.PrettyPrint(context)}";

        public static LambdaTerm Create(Term parameter, Term body) =>
            (parameter, body) switch
            {
                (UnspecifiedTerm _, UnspecifiedTerm _) => Unspecified,
                (KindTerm _, KindTerm _) => Kind,
                _ => new LambdaTerm(parameter, body)
            };

        public static new readonly LambdaTerm Unspecified =
            new LambdaTerm(UnspecifiedTerm.Instance, UnspecifiedTerm.Instance);
        public static new readonly LambdaTerm Kind =
            new LambdaTerm(KindTerm.Instance, KindTerm.Instance);
    }
}
