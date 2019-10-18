using System;

namespace Favalon.Expressions
{
    public sealed class LambdaExpression : Expression
    {
        private Expression higherOrder;
        
        public readonly Expression Parameter;
        public readonly Expression Body;

        public LambdaExpression(Expression parameter, Expression body)
        {
            this.Parameter = parameter;
            this.Body = body;

            higherOrder = ComputeHigherOrder(parameter, body) ??
                UnspecifiedExpression.Instance;
        }

        public override Expression HigherOrder =>
            higherOrder;

        public override bool Equals(Expression? rhs) =>
            rhs is LambdaExpression lambda ?
                (this.Parameter.Equals(lambda.Parameter) && this.Body.Equals(lambda.Body)) :
                false;

        private static Expression? ComputeHigherOrder(Expression parameter, Expression body) =>
            (parameter.HigherOrder, body.HigherOrder) switch
            {
                (UnspecifiedExpression _, UnspecifiedExpression _) => UnspecifiedExpression.Instance,
                (Expression lhs, UnspecifiedExpression _) => lhs,
                (UnspecifiedExpression _, Expression rhs) => rhs,
                _ => null
            };

        protected override Expression VisitResolve(IInferContext context)
        {
            var parameter = this.Parameter.VisitResolveCore(context);
            var body = this.Body.VisitResolveCore(context);

            if (ComputeHigherOrder(parameter, body) is Expression higherOrder)
            {
                this.higherOrder = higherOrder;
            }
            else
            {
                throw new Exception();
            }

            return this;
        }

        public override string ToString() =>
            $"{this.Parameter} -> {this.Body}";
    }
}
