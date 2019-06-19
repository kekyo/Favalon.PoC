namespace Favalon.Expressions
{
    public sealed class ApplyExpression : Expression
    {
        // f x
        public readonly Expression Function;
        public readonly Expression Argument;

        private ApplyExpression(Expression function, Expression argument, Expression higherOrder) :
            base(higherOrder)
        {
            this.Function = function;
            this.Argument = argument;
        }

        internal ApplyExpression(Expression function, Expression argument) :
            base((function.HigherOrder as FunctionExpression)?.Result ?? function.HigherOrder)
        {
            this.Function = function;
            this.Argument = argument;
        }

        internal override bool CanProduceSafeReadableString =>
            false;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            (!this.Argument.CanProduceSafeReadableString && this.Argument is ApplyExpression) ?
                $"{this.Function.GetReadableString(withAnnotation)} ({this.Argument.GetReadableString(withAnnotation)})" :
                $"{this.Function.GetReadableString(withAnnotation)} {this.Argument.GetReadableString(withAnnotation)}";

        internal override Expression Visit(ExpressionEnvironment environment)
        {
            var function = this.Function.Visit(environment);
            var argument = this.Argument.Visit(environment);

            // f:'a x:int  (f = int -> 'b)
            var resultHigherOrder = environment.CreatePlaceholder();

            var functionHigherOrder = new FunctionExpression(argument.HigherOrder, resultHigherOrder);
            if (function is VariableExpression variable)
            {
                environment.SetHigherOrder(variable.Name, functionHigherOrder);
            }

            environment.UnifyExpression(function.HigherOrder, resultHigherOrder);

            function.HigherOrder = functionHigherOrder;

            return new ApplyExpression(function, argument, resultHigherOrder);
        }

        internal override void Resolve(ExpressionEnvironment environment)
        {
            this.Function.Resolve(environment);
            this.Argument.Resolve(environment);
        }
    }
}
