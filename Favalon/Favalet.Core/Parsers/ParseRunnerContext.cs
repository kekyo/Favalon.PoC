using System.Diagnostics;
using Favalet.Expressions;
using Favalet.Tokens;
using Favalet.Contexts;

namespace Favalet.Parsers
{
    public sealed class ParseRunnerContext
    {
        public readonly IParseRunnerFactory Factory;

        //public NumericalSignToken? PreSignToken;

        [DebuggerStepThrough]
        private ParseRunnerContext(IParseRunnerFactory factory)
        {
            this.Factory = factory;
            //this.PreSignToken = null;
            this.LastToken = null;
        }

        public IExpression? Current { get; private set; }
        public Token? LastToken { get; private set; }

        [DebuggerStepThrough]
        internal void SetLastToken(Token token) =>
            this.LastToken = token;

        [DebuggerStepThrough]
        private static IExpression Combine(IExpression? left, IExpression? right)
        {
            if (left != null)
            {
                if (right != null)
                {
                    return ApplyExpression.Create(left, right);
                }
                else
                {
                    return left;
                }
            }
            else
            {
                return right!;
            }
        }

        [DebuggerStepThrough]
        public void CombineAfter(IExpression expression) =>
            this.Current = Combine(this.Current, expression);
        [DebuggerStepThrough]
        public void CombineBefore(IExpression expression) =>
            this.Current = Combine(expression, this.Current);

        [DebuggerStepThrough]
        public override string ToString() =>
            this.Current is IExpression current ?
                $"{current.GetPrettyString(PrettyStringTypes.Readable)}" :
                "(empty)";

        [DebuggerStepThrough]
        public static ParseRunnerContext Create(IParseRunnerFactory factory) =>
            new ParseRunnerContext(factory);
    }
}
