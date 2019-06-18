﻿using System;

namespace Favalon.Expressions
{
    public sealed class LambdaExpression : Expression
    {
        // x -> expr
        public readonly Expression Parameter;
        public readonly Expression Expression;

        internal LambdaExpression(Expression parameter, Expression expression) :
            base(expression.HigherOrder)
        {
            this.Parameter = parameter;
            this.Expression = expression;
        }

        public override string ReadableString =>
            $"{this.Parameter.ReadableString} -> {this.Expression.ReadableString}";

        internal override Expression Visit(ExpressionEnvironment environment)
        {
            var scoped = environment.NewScope();

            var parameter = this.Parameter.Visit(scoped);
            var expression = this.Expression.Visit(scoped);

            return new LambdaExpression(parameter, expression);
        }

        internal override void Resolve(ExpressionEnvironment environment)
        {
            this.Parameter.Resolve(environment);
            this.Expression.Resolve(environment);
        }
    }
}
