using Favalon.Contexts;

namespace Favalon.Terms
{
    public sealed class BindExpressionTerm : HigherOrderLazyTerm
    {
        public readonly Term Bound;
        public readonly Term Body;

        internal BindExpressionTerm(Term bound, Term body)
        {
            this.Bound = bound;
            this.Body = body;
        }

        protected override Term GetHigherOrder() =>
            this.Body.HigherOrder;

        public override Term Infer(InferContext context, Term higherOrderHint)
        {
            var higherOrder = this.Body.HigherOrder.Infer(context, higherOrderHint.HigherOrder);
            higherOrder = context.Unify(higherOrder, higherOrderHint).Term;

            var body = this.Body.Infer(context, higherOrderHint);
            var bound = this.Bound.Infer(context, higherOrderHint);

            if (bound is IdentityTerm(string identity))
            {
                context.SetBoundTerm(identity, body);
            }

            context.Unify(bound.HigherOrder, higherOrderHint);
            context.Unify(body.HigherOrder, higherOrderHint);

            return
                object.ReferenceEquals(bound, this.Bound) &&
                object.ReferenceEquals(body, this.Body) ?
                    this :
                    new BindExpressionTerm(bound, body);
        }

        public override Term Fixup(FixupContext context)
        {
            var body = this.Body.Fixup(context);
            var bound = this.Bound.Fixup(context);

            return
                object.ReferenceEquals(bound, this.Bound) &&
                object.ReferenceEquals(body, this.Body) ?
                    this :
                    new BindExpressionTerm(bound, body);
        }

        public override Term Reduce(ReduceContext context)
        {
            var body = this.Body.Reduce(context);
            var bound = this.Bound.Reduce(context);

            if (bound is IdentityTerm(string identity))
            {
                context.SetBoundTerm(identity, body);
            }

            return bound;
        }

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

    public sealed class BindTerm : HigherOrderLazyTerm
    {
        public readonly Term Expression;
        public readonly Term Continuation;

        internal BindTerm(Term expression, Term continuation)
        {
            this.Expression = expression;
            this.Continuation = continuation;
        }

        protected override Term GetHigherOrder() =>
            this.Continuation.HigherOrder;

        public override Term Infer(InferContext context, Term higherOrderHint)
        {
            var higherOrder = this.Continuation.HigherOrder.Infer(context, higherOrderHint.HigherOrder);
            higherOrder = context.Unify(higherOrder, higherOrderHint).Term;

            var newScope = context.NewScope();

            var expression = this.Expression.Infer(newScope, this.Expression.HigherOrder);
            var continuation = this.Continuation.Infer(newScope, higherOrderHint);

            context.Unify(continuation.HigherOrder, higherOrder);

            return
                object.ReferenceEquals(expression, this.Expression) &&
                object.ReferenceEquals(continuation, this.Continuation) ?
                    this :
                    new BindTerm(expression, continuation);
        }

        public override Term Fixup(FixupContext context)
        {
            var expression = this.Expression.Fixup(context);
            var continuation = this.Continuation.Fixup(context);

            return
                object.ReferenceEquals(expression, this.Expression) &&
                object.ReferenceEquals(continuation, this.Continuation) ?
                    this :
                    new BindTerm(expression, continuation);
        }

        public override Term Reduce(ReduceContext context)
        {
            var newScope = context.NewScope();

            var _ = this.Expression.Reduce(newScope);

            return this.Continuation.Reduce(newScope);
        }

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
