using Favalon.Contexts;
using System;

#pragma warning disable 659

namespace Favalon
{
    public abstract partial class Term : IEquatable<Term?>
    {
        protected Term()
        { }

        public abstract Term HigherOrder { get; }

        public abstract Term Infer(InferContext context);

        public abstract Term Fixup(FixupContext context);

        public abstract Term Reduce(ReduceContext context);

        public abstract bool Equals(Term? other);

        bool IEquatable<Term?>.Equals(Term? other) =>
            this.Equals(other);

        public override sealed bool Equals(object? other) =>
            this.Equals(other as Term);

        public void Deconstruct(out Term higherOrder) =>
            higherOrder = this.HigherOrder;
    }
}
