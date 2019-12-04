using System;
using System.Collections.Generic;

namespace LambdaCalculus
{
    public abstract class TypeTerm : Term
    {
        private static readonly Term higherOrder =
            new IdentityTerm("*", UnspecifiedTerm.Instance);  // TODO:

        private protected TypeTerm()
        { }

        public override Term HigherOrder =>
            higherOrder;

        public override Term Reduce(ReduceContext context) =>
            this;

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(InferContext context) =>
            this;
    }

    public sealed class ClrTypeTerm : TypeTerm
    {
        private static readonly Dictionary<Type, ClrTypeTerm> types =
            new Dictionary<Type, ClrTypeTerm>();

        private static readonly ClrTypeTerm higherOrder =
            new ClrTypeTerm(typeof(Type));

        public new readonly Type Type;

        private ClrTypeTerm(Type type) =>
            this.Type = type;

        public override Term HigherOrder =>
           higherOrder;

        public override Term Reduce(ReduceContext context) =>
            this;

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(InferContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is ClrTypeTerm rhs ? this.Type.Equals(rhs.Type) : false;

        public static ClrTypeTerm From(Type type)
        {
            if (!types.TryGetValue(type, out var term))
            {
                term = new ClrTypeTerm(type);
                types.Add(type, term);
            }
            return term;
        }
    }
}
