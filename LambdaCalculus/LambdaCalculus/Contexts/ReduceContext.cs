using System.Collections.Generic;

namespace Favalon.Contexts
{
    public sealed class ReduceContext : Context
    {
        internal int Iterations;

        private ReduceContext(ReduceContext parent) :
            base(parent) =>
            this.Iterations = parent.Iterations;

        internal ReduceContext(
            Context parent,
            Dictionary<string, Term> boundTerms,
            int iterations) :
            base(parent, boundTerms) =>
            this.Iterations = iterations;

        public ReduceContext NewScope() =>
            new ReduceContext(this);
    }
}
