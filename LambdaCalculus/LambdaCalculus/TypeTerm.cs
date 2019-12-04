using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LambdaCalculus
{
    public abstract class TypeTerm : Term
    {
        private static readonly Term higherOrder =
            new IdentityTerm("*", LambdaCalculus.UnspecifiedTerm.Instance);  // TODO:
        private static readonly Dictionary<Type, TypeTerm> clrTypes =
            new Dictionary<Type, TypeTerm>();

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

        private protected static bool IsTypeConstructor(Type type) =>
            type.IsGenericTypeDefinition && (type.GetGenericArguments().Length == 1);

        public static TypeTerm From(Type type)
        {
            if (!clrTypes.TryGetValue(type, out var term))
            {
                term = IsTypeConstructor(type) ?
                    (TypeTerm)new ClrTypeConstructorTerm(type) :
                    new ClrTypeTerm(type);
                clrTypes.Add(type, term);
            }
            return term;
        }
    }

    internal interface IClrType
    {
        Type Type { get; }
    }

    public sealed class ClrTypeTerm : TypeTerm, IClrType
    {
        internal static readonly Term higherOrder =
            From(typeof(Type));

        internal ClrTypeTerm(Type type)
        {
            Debug.Assert(!IsTypeConstructor(type));
            this.Type = type;
        }

        public new Type Type { get; }

        public override Term HigherOrder =>
           higherOrder;

        public override bool Equals(Term? other) =>
            other is ClrTypeTerm rhs ? this.Type.Equals(rhs.Type) : false;
    }

    public sealed class ClrTypeConstructorTerm : TypeTerm, IApplicable, IClrType
    {
        private static readonly Term higherOrder =
            new LambdaTerm(ClrTypeTerm.higherOrder, ClrTypeTerm.higherOrder);

        internal ClrTypeConstructorTerm(Type type)
        {
            Debug.Assert(IsTypeConstructor(type));
            this.Type = type;
        }

        public override Term HigherOrder =>
            higherOrder;

        public new Type Type { get; }

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
            From(this.Type.MakeGenericType(((IClrType)rhs.Reduce(context)).Type));

        public override bool Equals(Term? other) =>
            other is ClrTypeConstructorTerm rhs ? this.Type.Equals(rhs.Type) : false;
    }
}
