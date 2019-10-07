using Favalon.Expressions;
using Favalon.Internals;
using Favalon.Terms;

namespace Favalon
{
    public sealed class Inferrer
    {
        private readonly InferContext inferContext =
            new InferContext();

        public void Add(string symbolName, Expression expression) =>
            inferContext.Add(symbolName, expression);

        public Expression Infer(Term term)
        {
            var partial = term.VisitInferCore(inferContext);
            return partial.VisitResolveCore(inferContext);
        }
    }
}
