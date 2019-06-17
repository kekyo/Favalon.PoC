using System;

namespace Favalon.Expressions
{
    public sealed class ApplyExpression : Expression, IResolvedExpression
    {
        public readonly Expression Function;
        public readonly Expression Argument;

        internal ApplyExpression(Expression function, Expression argument)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public override string ReadableString =>
            (this.Argument is ApplyExpression) ?
            $"{this.Function.ReadableString} ({this.Argument.ReadableString})" :
            $"{this.Function.ReadableString} {this.Argument.ReadableString}";

        public Expression HigherOrderExpression =>
            (this.Function is IResolvedExpression re) ? re.HigherOrderExpression : null!;

        public override Expression Infer(ExpressionEnvironment environment)
        {
            var function = this.Function.Infer(environment);
            var argument = this.Argument.Infer(environment);



            return this;
        }
    }
}
