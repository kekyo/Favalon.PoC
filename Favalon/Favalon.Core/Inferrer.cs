using Favalon.Internals;
using Favalon.Terms;
using System.Collections.Generic;

namespace Favalon
{
    public sealed class Inferrer
    {
        private readonly InferrerCore inferrer = new InferrerCore();

        public void AddVariable(string variable, Term term) =>
            inferrer.AddVariable(variable, term);

        public IEnumerable<Term> Infer(IEnumerable<Token> tokens, bool withFlush = true)
        {
            foreach (var token in tokens)
            {
                foreach (var term in inferrer.Examine(token))
                {
                    yield return term;
                }
            }

            if (withFlush)
            {
                foreach (var term in inferrer.Flush())
                {
                    yield return term;
                }
            }
        }

        public IEnumerable<Term> Flush() =>
            inferrer.Flush();
    }
}
