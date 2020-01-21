using System.Collections.Generic;
using System.Linq;

namespace Favalon.TermContexts
{
    public sealed class ReduceContext : Context
    {
        private ReduceContext(ReduceContext parent) :
            base(parent)
        { }

        internal ReduceContext(Context parent, Dictionary<string, Term> boundTerms) :
            base(parent, boundTerms)
        { }

        public ReduceContext NewScope() =>
            new ReduceContext(this);

        public Term ReduceAll(Term term) =>
            base.InternalEnumerableReduce(term).Last();
    }
}
