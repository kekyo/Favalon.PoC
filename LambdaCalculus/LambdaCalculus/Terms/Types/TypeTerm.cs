using Favalon.Contexts;
using Favalon.Terms.Algebric;
using LambdaCalculus.Contexts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalon.Terms.Types
{
    public abstract class TypeTerm : Term
    {
        private static readonly Dictionary<Type, Term> clrTypes =
            new Dictionary<Type, Term>();

        private protected TypeTerm()
        { }

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

        public static Term? Narrow(Term lhs, Term rhs)
        {
            switch ((lhs, rhs))
            {
                // int: int <-- int
                // IComparable: IComparable <-- IComparable
                // _[1]: _[1] <-- _[1]
                case (_, _) when object.ReferenceEquals(lhs, rhs) || lhs.Equals(rhs):
                    return lhs;

                // object: object <-- int
                // IComparable: IComparable <-- string
                case (ClrTypeTerm lhsType, ClrTypeTerm rhsTerm):
                    return lhsType.Type.IsAssignableFrom(rhsTerm.Type) ?
                        lhsType :
                        null;

                // _[1]: _[1] <-- _[2]
                case (PlaceholderTerm placeholder, PlaceholderTerm _):
                    return placeholder;

                // _: _ <-- int
                // _: _ <-- (int | double)
                case (PlaceholderTerm placeholder, _):
                    return placeholder;

                // (int | double): (int | double) <-- (int | double)
                // (int | double | string): (int | double | string) <-- (int | double)
                // (int | IComparable): (int | IComparable) <-- (int | string)
                // null: (int | double) <-- (int | double | string)
                // null: (int | IServiceProvider) <-- (int | double)
                // (int | _): (int | _) <-- (int | string)
                // (_[1] | _[2]): (_[1] | _[2]) <-- (_[2] | _[1])
                case (SumTerm(Term[] lhsTerms), SumTerm(Term[] rhsTerms)):
                    var terms1 = rhsTerms.
                        Select(rhsTerm => lhsTerms.Any(lhsTerm => Narrow(lhsTerm, rhsTerm) != null)).
                        ToArray();
                    return terms1.All(term => term) ?
                        lhs :
                        null;

                // null: int <-- (int | double)
                case (_, SumTerm(Term[] rhsTerms)):
                    var terms2 = rhsTerms.
                        Select(rhsTerm => Narrow(lhs, rhsTerm)).
                        ToArray();
                    return terms2.All(term => term != null) ?
                        new SumTerm(terms2!) :
                        null;

                // (int | double): (int | double) <-- int
                // (int | IServiceProvider): (int | IServiceProvider) <-- int
                // (int | IComparable): (int | IComparable) <-- string
                // (int | _): (int | _) <-- string
                // (int | _[1]): (int | _[1]) <-- _[2]
                case (SumTerm(Term[] lhsTerms), _):
                    var terms3 = lhsTerms.
                        Select(lhsTerm => Narrow(lhsTerm, rhs)).
                        ToArray();
                    // Requirements: 1 or any terms narrowed.
                    if (terms3.Any(term => term != null))
                    {
                        terms3 = terms3.
                            Zip(lhsTerms, (term, lhsTerm) => term ?? lhsTerm).
                            ToArray();
                        return terms3.Length switch
                        {
                            0 => null,
                            1 => terms3[0],
                            _ => new SumTerm(terms3!)
                        };
                    }
                    // Couldn't narrow: (int | double) <-- string
                    else
                    {
                        return null;
                    }

                // null: int <-- _   [TODO: maybe]
                case (_, PlaceholderTerm placeholder):
                    return null;

                default:
                    return null;
            }
        }

        public static readonly IComparer<Term> ConcreterComparer =
            new ConcreterComparerImpl();

        private sealed class ConcreterComparerImpl : IComparer<Term>
        {
            private int Compare(Type x, Type y)
            {
                if (x.Equals(y))
                {
                    return 0;
                }
                else if (x.IsPrimitive && !y.IsPrimitive)
                {
                    return -1;
                }
                else if (!x.IsPrimitive && y.IsPrimitive)
                {
                    return 1;
                }
                else if (x.IsPrimitive && y.IsPrimitive)
                {
                    var cx = x.IsClsCompliant();
                    var cy = y.IsClsCompliant();
                    if (cx && !cy)
                    {
                        return -1;
                    }
                    else if (!cx && cy)
                    {
                        return -1;
                    }

                    var ix = x.IsInteger();
                    var iy = y.IsInteger();
                    if (ix && !iy)
                    {
                        return -1;
                    }
                    else if (!ix && iy)
                    {
                        return -1;
                    }

                    var sx = x.SizeOf();
                    var sy = y.SizeOf();
                    if (sx < sy)
                    {
                        return -1;
                    }
                    else if (sx > sy)
                    {
                        return 1;
                    }
                }
                else if (x.IsValueType && !y.IsValueType)
                {
                    return -1;
                }
                else if (!x.IsValueType && y.IsValueType)
                {
                    return 1;
                }
                else if (y.IsAssignableFrom(x))
                {
                    return -1;
                }
                else if (x.IsAssignableFrom(y))
                {
                    return 1;
                }

                return -1;
            }

            public int Compare(Term x, Term y) =>
                (x, y) switch
                {
                    (ClrTypeTerm(Type tx), ClrTypeTerm(Type ty)) => this.Compare(tx, ty),
                    (ClrTypeTerm(_), _) => -1,
                    (_, ClrTypeTerm(_)) => 1,
                    _ => 0
                };
        }

        public static readonly ClrTypeTerm Void =
            (ClrTypeTerm)From(typeof(void));

        public static readonly ClrTypeTerm Unit =
            (ClrTypeTerm)From(typeof(Unit));
    }

    internal interface IClrType
    {
        Type Type { get; }
    }

    public sealed class ClrTypeTerm : TypeTerm, IClrType
    {
        internal ClrTypeTerm(Type type)
        {
            Debug.Assert(!IsTypeConstructor(type));
            this.Type = type;
        }

        public override Term HigherOrder =>
            KindTerm.Instance;

        public new Type Type { get; }

        public override bool Equals(Term? other) =>
            other is ClrTypeTerm rhs ? this.Type.Equals(rhs.Type) : false;

        public override int GetHashCode() =>
            this.Type.GetHashCode();

        public void Deconstruct(out Type type) =>
            type = this.Type;

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            $"{this.Type.PrettyPrint(context)}";
    }

    public sealed class ClrTypeConstructorTerm : TypeTerm, IApplicable, IClrType
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
