using Favalon.Contexts;
using System;

namespace Favalon.Terms.Types
{
    public interface ITypeTerm : IComparable<ITypeTerm>
    {
    }

    public abstract class TypeTerm : Term, ITypeTerm
    {
        protected TypeTerm()
        { }

        public abstract int CompareTo(ITypeTerm other);

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;
    }
}
