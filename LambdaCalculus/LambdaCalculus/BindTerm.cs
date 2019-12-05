namespace LambdaCalculus
{
    public sealed class BindTerm : Term
    {
        public readonly Term Bound;
        public readonly Term Body;
        public readonly Term Continuation;

        internal BindTerm(Term bound, Term body, Term continuation)
        {
            this.Bound = bound;
            this.Body = body;
            this.Continuation = continuation;
        }

        public override Term HigherOrder =>
            this.Continuation.HigherOrder;

        public override Term Reduce(ReduceContext context)
        {
            var newScope = context.NewScope();

            var body = this.Body.Reduce(newScope);
            var bound = this.Bound.Reduce(newScope);

            newScope.AddBoundTerm(((IdentityTerm)bound).Identity, body);

            return this.Continuation.Reduce(newScope);
        }

        public override Term Infer(InferContext context)
        {
            var newScope = context.NewScope();

            var body = this.Body.Infer(newScope);
            var bound = this.Bound.Infer(newScope);

            newScope.AddBoundTerm(((IdentityTerm)bound).Identity, body);
            newScope.Unify(bound.HigherOrder, body.HigherOrder);

            var continuation = this.Continuation.Infer(newScope);
            return new BindTerm(bound, body, continuation);
        }

        public override Term Fixup(FixupContext context) =>
            new BindTerm(this.Bound.Fixup(context), this.Body.Fixup(context), this.Continuation.Fixup(context));

        public override bool Equals(Term? other) =>
            other is LambdaTerm rhs ?
                (this.Bound.Equals(rhs.Parameter) && this.Body.Equals(rhs.Body)) :
                false;

        public override int GetHashCode() =>
            this.Bound.GetHashCode() ^ this.Body.GetHashCode();

        public void Deconstruct(out Term parameter, out Term body)
        {
            parameter = this.Bound;
            body = this.Body;
        }

        public static new readonly LambdaTerm Unspecified =
            new LambdaTerm(UnspecifiedTerm.Instance, UnspecifiedTerm.Instance);
    }
}
