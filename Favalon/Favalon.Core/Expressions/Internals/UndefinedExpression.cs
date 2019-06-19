namespace Favalon.Expressions.Internals
{
    internal sealed class UndefinedExpression : Expression
    {
        // Dead end expression.

        internal UndefinedExpression() :
            base(null!)
        { }

        internal override string GetInternalReadableString(bool withAnnotation) =>
            "(Undefined)";

        internal override Expression Visit(ExpressionEnvironment environment) =>
            this;

        internal override void Resolve(ExpressionEnvironment environment)
        {
        }

        internal static readonly UndefinedExpression Instance = new UndefinedExpression();

        static UndefinedExpression() =>
            Instance.HigherOrder = Instance;
    }
}
