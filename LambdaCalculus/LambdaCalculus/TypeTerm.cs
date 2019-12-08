﻿using Favalon.AlgebricData;
using Favalon.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalon
{
    public abstract class TypeTerm : Term
    {
        private static readonly Dictionary<Type, TypeTerm> clrTypes =
            new Dictionary<Type, TypeTerm>();

        private protected TypeTerm()
        { }

        public override Term HigherOrder =>
            UnspecifiedTerm.Instance;

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
    }

    public sealed class DeclareTypeTerm : TypeTerm
    {
        public readonly Term Declare;

        internal DeclareTypeTerm(Term declare, Term higherOrder)
        {
            this.Declare = declare;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

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
                                reduced.All(item => item is PairTerm) ?
                                    (Term)new DiscriminatedUnionTerm(reduced.Cast<PairTerm>(), higherOrder) :
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

        public override int GetHashCode() =>
            this.Type.GetHashCode();
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

        public override int GetHashCode() =>
            this.Type.GetHashCode();
    }
}
