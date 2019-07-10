using Favalet.Expressions.Specialized;

namespace Favalet.Expressions
{
    public sealed class ApplyExpression : Expression
    {
        internal ApplyExpression(Expression function, Expression argument, Expression higherOrder) :
            base(higherOrder)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public readonly Expression Function;
        public readonly Expression Argument;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            FormattedString.RequiredEnclosing(
                $"{FormatReadableString(context, this.Function, true)} {FormatReadableString(context, this.Argument, true)}");

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint)
        {
            var higherOrder = environment.Unify(higherOrderHint, this.HigherOrder);

            var argument = environment.Visit(this.Argument, UnspecifiedExpression.Instance);
            var function = environment.Visit(this.Function, LambdaExpression.Create(argument.HigherOrder, higherOrder));

            return new ApplyExpression(function, argument, higherOrder);
        }

        protected override Expression VisitResolving(IResolvingEnvironment environment)
        {
            var argument = environment.Visit( this.Argument);
            var function = environment.Visit(this.Function);
            var higherOrder = environment.Visit(this.HigherOrder);

            return new ApplyExpression(function, argument, higherOrder);
        }
    }
}
