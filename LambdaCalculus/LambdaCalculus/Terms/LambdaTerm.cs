using Favalon.Contexts;

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
            new LambdaTerm(this.Parameter.HigherOrder, this.Body.HigherOrder);

        public override Term Infer(InferContext context)
        {
            // Best effort infer procedure.

            var newScope = context.NewScope();

            var parameter = this.Parameter.Infer(newScope);
            if (parameter is IdentityTerm identity)
            {
                // Shadowed by self.
                newScope.SetBoundTerm(identity.Identity, parameter);
            }

            // Calculate inferring with parameter identity.
            var body = this.Body.Infer(newScope);

            return
                object.ReferenceEquals(parameter, this.Parameter) &&
                object.ReferenceEquals(body, this.Body) ?
                    this :
                    new LambdaTerm(parameter, body);
        }

        Term IApplicable.InferForApply(InferContext context, Term rhs)
        {
            // Strict infer procedure.

            var newScope = context.NewScope();

            var parameter = this.Parameter.Infer(newScope);
            if (parameter is IdentityTerm identity)
            {
                // Applied argument.
                var argument = rhs.Infer(context);
                newScope.SetBoundTerm(identity.Identity, argument);
            }

            // Calculate inferring with applied argument.
            var body = this.Body.Infer(newScope);

            return
                object.ReferenceEquals(parameter, this.Parameter) &&
                object.ReferenceEquals(body, this.Body) ?
                    this :
                    new LambdaTerm(parameter, body);
        }

        public override Term Fixup(FixupContext context)
        {
            var parameter = this.Parameter.Fixup(context);
            var body = this.Body.Fixup(context);

            return
                object.ReferenceEquals(parameter, this.Parameter) &&
                object.ReferenceEquals(body, this.Body) ?
                    this :
                    new LambdaTerm(parameter, body);
        }

        public override Term Reduce(ReduceContext context)
        {
            var newScope = context.NewScope();

            var parameter = this.Parameter.Reduce(context);
            if (parameter is IdentityTerm identity)
            {
                // Shadowed by self
                newScope.SetBoundTerm(identity.Identity, identity);
            }

            var body = this.Body.Reduce(context);

            return
                object.ReferenceEquals(parameter, this.Parameter) &&
                object.ReferenceEquals(body, this.Body) ?
                    this :
                    new LambdaTerm(parameter, body);
        }

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs)
        {
            var newScope = context.NewScope();

            // It'll maybe make identity because already reduced by previous called Reduce().
            if (this.Parameter is IdentityTerm identity)
            {
                var argument = rhs.Reduce(context);
                newScope.SetBoundTerm(identity.Identity, argument);
                return this.Body.Reduce(newScope);
            }
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

        public static LambdaTerm Create(Term parameter, Term body)
        {
            if (parameter is UnspecifiedTerm && body is UnspecifiedTerm)
            {
                return Unspecified;
            }
            else if (parameter is KindTerm && body is KindTerm)
            {
                return Kind;
            }
            else
            {
                return new LambdaTerm(parameter, body);
            }
        }

        public static new readonly LambdaTerm Unspecified =
            new LambdaTerm(UnspecifiedTerm.Instance, UnspecifiedTerm.Instance);
        public static new readonly LambdaTerm Kind =
            new LambdaTerm(KindTerm.Instance, KindTerm.Instance);
    }
}
