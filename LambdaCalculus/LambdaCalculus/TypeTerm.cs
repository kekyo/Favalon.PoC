using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LambdaCalculus
{
    public abstract class TypeTerm : Term
    {
        private static readonly Term higherOrder =
            new IdentityTerm("*", LambdaCalculus.UnspecifiedTerm.Instance);  // TODO:
        private static readonly Dictionary<Type, ClrTypeTerm> types =
            new Dictionary<Type, ClrTypeTerm>();

        private protected TypeTerm()
        { }

        public override Term HigherOrder =>
            higherOrder;

        public override sealed Term Reduce(ReduceContext context) =>
            this;

        public override sealed Term Infer(InferContext context) =>
            this;

        public override sealed Term Fixup(InferContext context) =>
            this;

        public static TypeTerm From(Type type)
        {
            if (!types.TryGetValue(type, out var term))
            {
                term = new ClrTypeTerm(type);
                types.Add(type, term);
            }
            return term;
        }
    }

    public sealed class ClrTypeTerm : TypeTerm
    {
        private static readonly Term higherOrder =
            From(typeof(Type));

        public new readonly Type Type;

        internal ClrTypeTerm(Type type) =>
            this.Type = type;

        public override Term HigherOrder =>
           higherOrder;

        public override bool Equals(Term? other) =>
            other is ClrTypeTerm rhs ? this.Type.Equals(rhs.Type) : false;
    }
}
