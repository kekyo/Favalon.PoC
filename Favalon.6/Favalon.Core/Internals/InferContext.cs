using Favalon.Expressions;
using Favalon.Terms;
using System.Collections.Generic;

namespace Favalon.Internals
{
    internal sealed class InferContext : Term.IInferContext
    {
        private static readonly IReadOnlyList<Expression> empty = new Expression[0];

        private readonly Dictionary<string, List<Expression>> boundExpressions =
            new Dictionary<string, List<Expression>>();

        internal void Add(string symbolName, Expression expression)
        {
            if (!boundExpressions.TryGetValue(symbolName, out var expressions))
            {
                expressions = new List<Expression>();
                boundExpressions.Add(symbolName, expressions);
            }
            expressions.Add(expression);
        }

        public IReadOnlyList<Expression> Lookup(string symbolName) =>
            boundExpressions.TryGetValue(symbolName, out var expressions) ?
                expressions :
                empty;

        public void Unify(Expression expression1, Expression expression2)
        {
            if (expression1.Equals(expression2))
            {
                return;
            }


        }
    }
}
