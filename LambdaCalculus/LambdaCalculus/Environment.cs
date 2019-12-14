using Favalon.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace Favalon
{
    public sealed class Environment : Context
    {
        private Environment()
        { }

        public Term Infer(Term term)
        {
            var context = new InferContext(this);
            var partial = term.Infer(context);
            return partial.Fixup(context);
        }

        public IEnumerable<Term> EnumerableReduce(Term term)
        {
            var inferred = this.Infer(term);

            if (boundTerms == null)
            {
                boundTerms = new Dictionary<string, Term>();
            }

            var current = inferred;
            while (true)
            {
                yield return current;

                var context = new ReduceContext(this, boundTerms);

                var reduced = current.Reduce(context);
                if (object.ReferenceEquals(reduced, current))
                {
                    break;
                }

                current = reduced;
            }
        }

        public Term Reduce(Term term, bool oneTime = false) =>
            oneTime ?
                this.EnumerableReduce(term).First() :
                this.EnumerableReduce(term).Last();

        public static Environment Create() =>
            new Environment();
    }
}
