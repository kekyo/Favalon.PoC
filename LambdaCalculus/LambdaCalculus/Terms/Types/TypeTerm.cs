using Favalon.Contexts;
using System;

namespace Favalon.Terms.Types
{
    public abstract class TypeTerm : Term
    {
        protected TypeTerm()
        { }

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;
    }
}
