using Favalon.Expressions;
using Favalon.Internals;
using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;

namespace Favalon
{
    public sealed class Checker
    {
        private readonly Term.InferContext inferContext =
            new Term.InferContext();
        private readonly Dictionary<string, List<Expression>> boundExpressions =
            new Dictionary<string, List<Expression>>();

        public void Add(string variable, Expression expression)
        {
            if (!boundExpressions.TryGetValue(variable, out var expressions))
            {
                expressions = new List<Expression>();
                boundExpressions.Add(variable, expressions);
            }
            expressions.Add(expression);
        }

        public Expression Infer(Term term)
        {
            return term.VisitCore(inferContext);
        }
    }
}
