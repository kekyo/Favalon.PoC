using Favalon.Terms.Contexts;

namespace Favalon.Terms
{
    public sealed class BindExpressionTerm : HigherOrderLazyTerm
    {
        public readonly Term Bound;
        public readonly Term Body;

        private BindExpressionTerm(Term bound, Term body)
        {
            this.Bound = bound;
            this.Body = body;
        }

        protected override Term GetHigherOrder() =>
            this.Body.HigherOrder;

        public override Term Infer(InferContext context)
        {
            var body = this.Body.Infer(context);
            var bound = this.Bound.Infer(context);

            if (bound is IdentityTerm(string identity))
            {
                context.SetBindTerm(identity, body);
            }

            context.Unify(bound.HigherOrder, body.HigherOrder);

            return
                this.Bound.EqualsWithHigherOrder(bound) &&
                this.Body.EqualsWithHigherOrder(body) ?
                    this :
                    new BindExpressionTerm(bound, body);
        }

        public override Term Fixup(FixupContext context)
        {
            var body = this.Body.Fixup(context);
            var bound = this.Bound.Fixup(context);

            return
                this.Bound.EqualsWithHigherOrder(bound) &&
                this.Body.EqualsWithHigherOrder(body) ?
                    this :
                    new BindExpressionTerm(bound, body);
        }

        public override Term Reduce(ReduceContext context)
        {
            // Try reduce to finished.
            var body = context.ReduceAll(this.Body);

            var bound = this.Bound.Reduce(context);
            if (bound is IdentityTerm(string identity))
            {
                context.SetBindTerm(identity, body);
                return body;
            }

            return
                this.Bound.EqualsWithHigherOrder(bound) &&
                this.Body.EqualsWithHigherOrder(body) ?
                    this :
                    new BindExpressionTerm(bound, body);
        }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is BindExpressionTerm rhs ?
                (this.Bound.Equals(context, rhs.Bound) && this.Body.Equals(context, rhs.Body)) :
                false;

        public override int GetHashCode() =>
            this.Bound.GetHashCode() ^ this.Body.GetHashCode();

        public void Deconstruct(out Term bound, out Term body)
        {
            bound = this.Bound;
            body = this.Body;
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Bound.PrettyPrint(context)} = {this.Body.PrettyPrint(context)}";

        public static BindExpressionTerm Create(Term bound, Term body) =>
            new BindExpressionTerm(bound, body);
    }

    public sealed class BindTerm : HigherOrderLazyTerm
    {
        public readonly Term Expression;
        public readonly Term Continuation;

        private BindTerm(Term expression, Term continuation)
        {
            this.Expression = expression;
            this.Continuation = continuation;
        }

        protected override Term GetHigherOrder() =>
            this.Continuation.HigherOrder;

        public override Term Infer(InferContext context)
        {
            var newScope = context.NewScope();

            var expression = this.Expression.Infer(newScope);
            var continuation = this.Continuation.Infer(newScope);

            return
                this.Expression.EqualsWithHigherOrder(expression) &&
                this.Continuation.EqualsWithHigherOrder(continuation) ?
                    this :
                    new BindTerm(expression, continuation);
        }

        public override Term Fixup(FixupContext context)
        {
            var expression = this.Expression.Fixup(context);
            var continuation = this.Continuation.Fixup(context);

            return
                this.Expression.EqualsWithHigherOrder(expression) &&
                this.Continuation.EqualsWithHigherOrder(continuation) ?
                    this :
                    new BindTerm(expression, continuation);
        }

        public override Term Reduce(ReduceContext context)
        {
            var newScope = context.NewScope();

            var _ = this.Expression.Reduce(newScope);

            return this.Continuation.Reduce(newScope);
        }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is BindTerm rhs ?
                (this.Expression.Equals(context, rhs.Expression) && this.Continuation.Equals(context, rhs.Continuation)) :
                false;

        public override int GetHashCode() =>
            this.Expression.GetHashCode() ^ this.Continuation.GetHashCode();

        public void Deconstruct(out Term expression, out Term continuation)
        {
            expression = this.Expression;
            continuation = this.Continuation;
        }

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Expression.PrettyPrint(context)} in {this.Continuation.PrettyPrint(context)}";

        public static BindTerm Create(Term expression, Term continuation) =>
            new BindTerm(expression, continuation);
    }
}
