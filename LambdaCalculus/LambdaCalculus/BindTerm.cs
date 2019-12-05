namespace LambdaCalculus
{
    public sealed class BindExpressionTerm : Term
    {
        public readonly Term Bound;
        public readonly Term Body;

        internal BindExpressionTerm(Term bound, Term body)
        {
            this.Bound = bound;
            this.Body = body;
        }

        public override Term HigherOrder =>
            this.Body.HigherOrder;

        public override Term Reduce(ReduceContext context)
        {
            var body = this.Body.Reduce(context);
            var bound = this.Bound.Reduce(context);

            context.SetBoundTerm(((IdentityTerm)bound).Identity, body);

            return bound;
        }

        public override Term Infer(InferContext context)
        {
            var body = this.Body.Infer(context);
            var bound = this.Bound.Infer(context);

            context.SetBoundTerm(((IdentityTerm)bound).Identity, body);
            context.Unify(bound.HigherOrder, body.HigherOrder);

            return new BindExpressionTerm(bound, body);
        }

        public override Term Fixup(FixupContext context) =>
            new BindExpressionTerm(this.Bound.Fixup(context), this.Body.Fixup(context));

        public override bool Equals(Term? other) =>
            other is BindExpressionTerm rhs ?
                (this.Bound.Equals(rhs.Bound) && this.Body.Equals(rhs.Body)) :
                false;

        public override int GetHashCode() =>
            this.Bound.GetHashCode() ^ this.Body.GetHashCode();

        public void Deconstruct(out Term bound, out Term body)
        {
            bound = this.Bound;
            body = this.Body;
        }
    }

    public sealed class BindTerm : Term
    {
        public readonly Term Expression;
        public readonly Term Continuation;

        internal BindTerm(Term expression, Term continuation)
        {
            this.Expression = expression;
            this.Continuation = continuation;
        }

        public override Term HigherOrder =>
            this.Continuation.HigherOrder;

        public override Term Reduce(ReduceContext context)
        {
            var newScope = context.NewScope();

            this.Expression.Reduce(newScope);

            return this.Continuation.Reduce(newScope);
        }

        public override Term Infer(InferContext context)
        {
            var newScope = context.NewScope();

            var expression = this.Expression.Infer(newScope);
            var continuation = this.Continuation.Infer(newScope);
            return new BindTerm(expression, continuation);
        }

        public override Term Fixup(FixupContext context) =>
            new BindTerm(this.Expression.Fixup(context), this.Continuation.Fixup(context));

        public override bool Equals(Term? other) =>
            other is BindTerm rhs ?
                (this.Expression.Equals(rhs.Expression) && this.Continuation.Equals(rhs.Continuation)) :
                false;

        public override int GetHashCode() =>
            this.Expression.GetHashCode() ^ this.Continuation.GetHashCode();

        public void Deconstruct(out Term expression, out Term continuation)
        {
            expression = this.Expression;
            continuation = this.Continuation;
        }
    }
}
