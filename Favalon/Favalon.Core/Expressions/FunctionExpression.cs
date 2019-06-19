using System;

namespace Favalon.Expressions
{
    public sealed class FunctionExpression : Expression
    {
        // 'a -> 'b
        public readonly Expression Parameter;
        public readonly Expression Result;

        internal FunctionExpression(Expression parameter, Expression result) :
            base(KindExpression.Instance)
        {
            this.Parameter = parameter;
            this.Result = result;
        }

        internal override string GetInternalReadableString(bool withAnnotation) =>
            $"({this.Parameter.GetReadableString(withAnnotation)} -> {this.Result.GetReadableString(withAnnotation)})";

        internal override Expression Visit(ExpressionEnvironment environment)
        {
            var parameter = this.Parameter.Visit(environment);
            var result = this.Result.Visit(environment);

            return new FunctionExpression(parameter, result);
        }

        internal override void Resolve(ExpressionEnvironment environment)
        {
            this.Parameter.Resolve(environment);
            this.Result.Resolve(environment);
        }
    }
}
