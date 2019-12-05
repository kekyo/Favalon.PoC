namespace Favalon
{
    public sealed class LambdaTerm : Term, IApplicable
    {
        public readonly Term Parameter;
        public readonly Term Body;

        internal LambdaTerm(Term parameter, Term body)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        public override Term HigherOrder =>
            new LambdaTerm(this.Parameter.HigherOrder, this.Body.HigherOrder);

        public override Term Reduce(ReduceContext context) =>
            new LambdaTerm(this.Parameter.Reduce(context), this.Body.Reduce(context));

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs)
        {
            var newScope = context.NewScope();
            newScope.SetBoundTerm(((IdentityTerm)this.Parameter).Identity, rhs);

            return this.Body.Reduce(newScope);
        }

        public override Term Infer(InferContext context)
        {
            var newScope = context.NewScope();
            var parameter = this.Parameter.Infer(newScope);
            if (parameter is IdentityTerm identity)
            {
                newScope.SetBoundTerm(identity.Identity, parameter);
            }

            var body = this.Body.Infer(newScope);

            return new LambdaTerm(parameter, body);
        }

        public override Term Fixup(FixupContext context) =>
            new LambdaTerm(this.Parameter.Fixup(context), this.Body.Fixup(context));

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

        public static new readonly LambdaTerm Unspecified =
            new LambdaTerm(UnspecifiedTerm.Instance, UnspecifiedTerm.Instance);
    }
}
