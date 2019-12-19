using Favalon.Contexts;
using Favalon.Terms.Algebric;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalon.Terms.Types
{
    public abstract class TypeTerm : Term
    {
        private static readonly Dictionary<Type, TypeTerm> clrTypes =
            new Dictionary<Type, TypeTerm>();

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

        public static DeclareTypeTerm From(Term declare, Term higherOrder) =>
            new DeclareTypeTerm(declare, higherOrder);

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

        public static readonly TypeTerm Void =
            From(typeof(void));

        public static readonly TypeTerm Unit =
            From(typeof(Unit));
    }

    public sealed class DeclareTypeTerm : HigherOrderHoldTerm
    {
        public readonly Term Declare;

        internal DeclareTypeTerm(Term declare, Term higherOrder) :
            base(higherOrder) =>
            this.Declare = declare;

        public override Term Infer(InferContext context)
        {
            switch (this.Declare)
            {
                // Discriminated union
                case SumTerm(Term[] items):
                    {
                        var inferred = items.Select(item => item.Infer(context)).ToArray();
                        var commonHigherOrder = context.CreatePlaceholder(UnspecifiedTerm.Instance);

                        foreach (var inferredItem in inferred)
                        {
                            context.Unify(inferredItem.HigherOrder, commonHigherOrder);
                        }

                        var higherOrder = this.HigherOrder.Infer(context);

                        return
                            object.ReferenceEquals(higherOrder, this.HigherOrder) &&
                            inferred.Zip(items, object.ReferenceEquals).All(r => r) ?
                                this :
                                new DeclareTypeTerm(new SumTerm(inferred), higherOrder);
                    }

                // TODO: Record
                default:
                    {
                        var declare = this.Declare.Infer(context);
                        var higherOrder = this.HigherOrder.Infer(context);

                        return
                            object.ReferenceEquals(declare, this.Declare) &&
                            object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                                this :
                                new DeclareTypeTerm(declare, higherOrder);
                    }
            }
        }

        public override Term Fixup(FixupContext context)
        {
            var declare = this.Declare.Fixup(context);
            var higherOrder = this.HigherOrder.Fixup(context);

            return
                object.ReferenceEquals(declare, this.Declare) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                    this :
                    new DeclareTypeTerm(declare, higherOrder);
        }

        public override Term Reduce(ReduceContext context)
        {
            switch (this.Declare)
            {
                // Discriminated union
                case SumTerm(Term[] items):
                    {
                        var reduced = items.Select(item => item.Reduce(context)).ToArray();
                        var higherOrder = this.HigherOrder.Reduce(context);

                        return
                            object.ReferenceEquals(higherOrder, this.HigherOrder) &&
                            reduced.Zip(items, object.ReferenceEquals).All(r => r) ?
                                (Term)this :
                                reduced.All(item => item is BindExpressionTerm) ?
                                    (Term)new DiscriminatedUnionTypeTerm(reduced, higherOrder) :
                                    new DeclareTypeTerm(this.Declare, higherOrder);

                    }

                // TODO: Record
                default:
                    {
                        var declare = this.Declare.Reduce(context);
                        var higherOrder = this.HigherOrder.Reduce(context);

                        return
                            object.ReferenceEquals(declare, this.Declare) &&
                            object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                                this :
                                new DeclareTypeTerm(declare, higherOrder);
                    }
            };
        }

        public override bool Equals(Term? other) =>
            other is DeclareTypeTerm rhs ? this.Declare.Equals(rhs.Declare) : false;
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

        Term IApplicable.InferForApply(InferContext context, Term rhs) =>
            this;

        Term? IApplicable.ReduceForApply(ReduceContext context, Term rhs) =>
            From(this.Type.MakeGenericType(((IClrType)rhs.Reduce(context)).Type));

        public override bool Equals(Term? other) =>
            other is ClrTypeConstructorTerm rhs ? this.Type.Equals(rhs.Type) : false;

        public override int GetHashCode() =>
            this.Type.GetHashCode();
    }
}
