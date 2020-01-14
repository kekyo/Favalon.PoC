using Favalon.Contexts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalon.Terms.Types
{
    public interface ITypeTerm : IComparable<ITypeTerm>
    {
        bool IsAssignableFrom(ITypeTerm fromType);
    }

    public abstract class TypeTerm : Term, ITypeTerm
    {
        private static readonly Dictionary<Type, Term> clrTypes =
            new Dictionary<Type, Term>();

        private protected TypeTerm()
        { }

        public abstract bool IsAssignableFrom(ITypeTerm fromType);

        public abstract int CompareTo(ITypeTerm other);

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        public override Term Reduce(ReduceContext context) =>
            this;

        private protected static bool IsTypeConstructor(Type type) =>
            type.IsGenericTypeDefinition && (type.GetGenericArguments().Length == 1);

        public static Term From(Type type)
        {
            if (!clrTypes.TryGetValue(type, out var term))
            {
                if (IsTypeConstructor(type))
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

        public static readonly ClrTypeTerm Void =
            (ClrTypeTerm)From(typeof(void));

        public static readonly ClrTypeTerm Unit =
            (ClrTypeTerm)From(typeof(Unit));
    }

    internal interface IClrTypeTerm : ITypeTerm
    {
        Type Type { get; }
    }

    internal static class TypeTermExtension
    {
        public static void Deconstruct(this IClrTypeTerm term, out Type type) =>
            type = term.Type;
    }

    public sealed class ClrTypeTerm : TypeTerm, IClrTypeTerm
    {
        internal ClrTypeTerm(Type type)
        {
            Debug.Assert(!IsTypeConstructor(type));
            this.Type = type;
        }

        public override Term HigherOrder =>
            KindTerm.Instance;

        public new Type Type { get; }

        public override bool IsAssignableFrom(ITypeTerm fromType) =>
            fromType switch
            {
                IClrTypeTerm f => this.Type.IsAssignableFrom(f.Type),   // TODO: must include sum calculation
                _ => false
            };

        public override int CompareTo(ITypeTerm other) =>
            other switch
            {
                IClrTypeTerm o => ClrTypeCalculator.WideningComparer.Compare(this, o)
            };

        public override bool Equals(Term? other) =>
            other is ClrTypeTerm rhs ? this.Type.Equals(rhs.Type) : false;

        public override int GetHashCode() =>
            this.Type.GetHashCode();

        public void Deconstruct(out Type type) =>
            type = this.Type;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Type.PrettyPrint(context)}";
    }

    public sealed class ClrTypeConstructorTerm : TypeTerm, IApplicable, IClrTypeTerm
    {
        internal ClrTypeConstructorTerm(Type type)
        {
            Debug.Assert(IsTypeConstructor(type));
            this.Type = type;
        }

        public override Term HigherOrder =>
            // * -> * (TODO: make nested kind lambda from flatten generic type arguments: * -> * -> * ...)
            LambdaTerm.Kind;

        public new Type Type { get; }

        Term IApplicable.InferForApply(InferContext context, Term argument, Term higherOrderHint) =>
            this;

        Term IApplicable.FixupForApply(FixupContext context, Term argument, Term higherOrderHint) =>
            this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint) =>
            From(this.Type.MakeGenericType(((IClrType)argument.Reduce(context)).Type));

        public override bool Equals(Term? other) =>
            other is ClrTypeConstructorTerm rhs ? this.Type.Equals(rhs.Type) : false;

        public override int GetHashCode() =>
            this.Type.GetHashCode();

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Type.PrettyPrint(context)}";
    }
}
