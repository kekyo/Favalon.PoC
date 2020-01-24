using Favalon.Terms.Contexts;
using System;
using System.Collections.Generic;

namespace Favalon.Terms.Types
{
    public sealed class ClrTypeTerm : Term, IValueTerm
    {
        private static readonly Dictionary<Type, Term> clrTypes =
            new Dictionary<Type, Term>();

        public readonly Type Type;

        private ClrTypeTerm(Type type) =>
            this.Type = type;

        public override Term HigherOrder =>
            KindTerm.Instance;

        object IValueTerm.Value =>
            this.Type;

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is ClrTypeTerm rhs ? this.Type.Equals(rhs.Type) : false;

        public override int GetHashCode() =>
            this.Type.GetHashCode();

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            this.Type.PrettyPrint(context);

        internal static Term From(Type type)
        {
            if (!clrTypes.TryGetValue(type, out var term))
            {
                if (Utilities.IsTypeConstructor(type))
                {
                    term = new ClrTypeConstructorTerm(type);
                }
                else
                {
                    term = new ClrTypeTerm(type);
                }
                clrTypes.Add(type, term);
            }
            return term;
        }
    }
}
