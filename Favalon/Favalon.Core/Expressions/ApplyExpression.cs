using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class ApplyExpression : Expression
    {
        // f x
        public Expression Function { get; private set; }
        public Expression Argument { get; private set; }

        private ApplyExpression(Expression function, Expression argument, Expression higherOrder) :
            base(higherOrder)
        {
            this.Function = function;
            this.Argument = argument;
        }

        internal ApplyExpression(Expression function, Expression argument) :
            base((function.HigherOrder as LambdaExpression)?.Expression ?? function.HigherOrder)
        {
            this.Function = function;
            this.Argument = argument;
        }

        internal override bool CanProduceSafeReadableString =>
            false;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            (!this.Argument.CanProduceSafeReadableString || this.Argument is ApplyExpression) ?
                $"{this.Function.GetReadableString(withAnnotation)} ({this.Argument.GetReadableString(withAnnotation)})" :
                $"{this.Function.GetReadableString(withAnnotation)} {this.Argument.GetReadableString(withAnnotation)}";

        protected internal override Expression Visit(Environment environment, InferContext context)
        {
            var function = this.Function.Visit(environment, context);
            var argument = this.Argument.Visit(environment, context);

            var resultHigherOrder = context.CreatePlaceholder(1);

            if (function is VariableExpression variable)
            {
                var variableHigherOrder = new LambdaExpression(argument.HigherOrder, resultHigherOrder);
                context.UnifyExpression(variable.HigherOrder, variableHigherOrder);
                environment.SetNamedExpression(variable.Name, variable);
            }
            else
            {
                context.UnifyExpression(function.HigherOrder, resultHigherOrder);
            }

            return new ApplyExpression(function, argument, resultHigherOrder);
        }

        protected internal override bool TraverseChildren(System.Func<Expression, Expression> yc)
        {
            this.Function = yc(this.Function);
            this.Argument = yc(this.Argument);
            return true;
        }
    }
}
