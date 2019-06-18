using System;

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

        public override string ReadableString =>
            (this.Argument is ApplyExpression) ?
            $"{this.Function.ReadableString} ({this.Argument.ReadableString})" :
            $"{this.Function.ReadableString} {this.Argument.ReadableString}";

        internal override Expression Visit(ExpressionEnvironment environment)
        {
            var function = this.Function.Visit(environment);
            var argument = this.Argument.Visit(environment);

            // f:'a x:int  (f = int -> 'b)
            var resultHigherOrder = environment.CreatePlaceholder();
            var ft = new FunctionExpression(argument.HigherOrder, resultHigherOrder);
            function.HigherOrder = ft;

            return new ApplyExpression(function, argument, resultHigherOrder);
        }

        internal override void Resolve(ExpressionEnvironment environment)
        {
            this.Function.Resolve(environment);
            this.Argument.Resolve(environment);
        }
    }
}
