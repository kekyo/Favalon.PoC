using System.Collections.Generic;

namespace Favalon.Contexts
{
    public sealed class ReduceContext : Context
    {
        internal ReduceContext(Context parent) :
            base(parent)
        { }

        internal ReduceContext(
            Context parent,
            Dictionary<string, Term> boundTerms) :
            base(parent, boundTerms)
        { }

        public ReduceContext NewScope() =>
            new ReduceContext(this);
    }
}
