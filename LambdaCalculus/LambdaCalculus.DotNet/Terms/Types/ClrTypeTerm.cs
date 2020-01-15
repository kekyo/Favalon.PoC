using Favalon.Contexts;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalon.Terms.Types
{
    internal interface IClrTypeTerm : ITypeTerm
    {
        Type Type { get; }
    }

    internal static class ClrTypeTermExtension
    {
        public static void Deconstruct(this IClrTypeTerm term, out Type type) =>
            type = term.Type;
    }

    public abstract class ClrTypeTermBase : TypeTerm, IClrTypeTerm
    {
        private static readonly Dictionary<Type, Term> clrTypes =
            new Dictionary<Type, Term>();
        private protected readonly Type type;

        internal ClrTypeTermBase(Type type)
        {
            Debug.Assert(!IsTypeConstructor(type));
            this.type = type;
        }

        Type IClrTypeTerm.Type =>
            type;

        public override bool IsAssignableFrom(ITypeTerm fromType) =>
            fromType switch
            {
                IClrTypeTerm f => type.IsAssignableFrom(f.Type),   // TODO: must include sum calculation
                _ => false
            };

        public override int CompareTo(ITypeTerm other) =>
            other switch
            {
                IClrTypeTerm o => ClrTypeCalculator.WideningComparer.Compare(this, o),
                _ => -1
            };

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

    public sealed class ClrTypeTerm : ClrTypeTermBase, IClrTypeTerm
    {
        internal ClrTypeTerm(Type type) :
            base(type)
        { }

        public override Term HigherOrder =>
            KindTerm.Instance;

        public override bool IsAssignableFrom(ITypeTerm fromType) =>
            fromType switch
            {
                IClrTypeTerm f => base.type.IsAssignableFrom(f.Type),   // TODO: must include sum calculation
                _ => false
            };

        public override int CompareTo(ITypeTerm other) =>
            other switch
            {
                IClrTypeTerm o => ClrTypeCalculator.WideningComparer.Compare(this, o),
                _ => -1
            };

        public override bool Equals(Term? other) =>
            other is ClrTypeTerm rhs ? base.type.Equals(rhs.type) : false;

        public override int GetHashCode() =>
            base.type.GetHashCode();

        public void Deconstruct(out Type type) =>
            type = base.type;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{base.type.PrettyPrint(context)}";
    }

    public sealed class ClrTypeConstructorTerm : ClrTypeTermBase, IClrTypeTerm, IApplicable
    {
        internal ClrTypeConstructorTerm(Type type) :
            base(type)
        { }

        public override Term HigherOrder =>
            // * -> * (TODO: make nested kind lambda from flatten generic type arguments: * -> * -> * ...)
            LambdaTerm.Kind;

        public override bool IsAssignableFrom(ITypeTerm fromType) =>
            fromType switch
            {
                IClrTypeTerm f => base.type.IsAssignableFrom(f.Type),   // TODO: must include sum calculation
                _ => false
            };

        public override int CompareTo(ITypeTerm other) =>
            other switch
            {
                IClrTypeTerm o => ClrTypeCalculator.WideningComparer.Compare(this, o),
                _ => -1
            };

        Term IApplicable.InferForApply(InferContext context, Term argument, Term higherOrderHint) =>
            this;

        Term IApplicable.FixupForApply(FixupContext context, Term argument, Term higherOrderHint) =>
            this;

        AppliedResult IApplicable.ReduceForApply(ReduceContext context, Term argument, Term higherOrderHint)
        {
            var argument_ = argument.Reduce(context);

            if (argument_ is IClrTypeTerm typeTerm)
            {
                var realType = base.type.MakeGenericType(typeTerm.Type);
                return AppliedResult.Applied(From(realType), argument_);
            }
            else
            {
                return AppliedResult.Ignored(this, argument_);
            }
        }

        public override bool Equals(Term? other) =>
            other is ClrTypeConstructorTerm rhs ? base.type.Equals(rhs.type) : false;

        public override int GetHashCode() =>
            base.type.GetHashCode();

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{base.type.PrettyPrint(context)}";
    }
}
