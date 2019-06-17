using System;

namespace Favalon.Expressions
{
    public sealed class BindExpression : Expression
    {
        public readonly VariableExpression Parameter;
        public readonly Expression Expression;
        public readonly Expression Body;

        internal BindExpression(VariableExpression parameter, Expression expression, Expression body)
        {
            this.Parameter = parameter;
            this.Expression = expression;
            this.Body = body;
        }

        public override string ReadableString =>
            $"{this.Parameter.ReadableString} = {this.Expression.ReadableString} in {this.Body.ReadableString}";

        public override Expression Infer(ExpressionEnvironment environment)
        {
            var scoped = environment.NewScope();

            var parameter = this.Parameter.Infer(scoped);



            return this;
        }
    }
}
