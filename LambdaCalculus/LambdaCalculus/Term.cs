using System;
using System.Collections.Generic;

namespace LambdaCalculus
{
    public abstract class Term :
        IEquatable<Term?>
    {
        public abstract Term HigherOrder { get; }

        public abstract Term Reduce(ReduceContext context);

        public abstract Term Infer(InferContext context);

        public abstract Term Fixup(InferContext context);

        public abstract bool Equals(Term? other);

        bool IEquatable<Term?>.Equals(Term? other) =>
            this.Equals(other);

        public override bool Equals(object? other) =>
            this.Equals(other as Term);

        //////////////////////////////////

        private static readonly Dictionary<Type, ClrTypeTerm> types =
            new Dictionary<Type, ClrTypeTerm>();

        public static ClrTypeTerm Type(Type type)
        {
            if (!types.TryGetValue(type, out var term))
            {
                term = new ClrTypeTerm(type);
                types.Add(type, term);
            }
            return term;
        }

        public static ClrTypeTerm Type<T>() =>
            Type(typeof(T));

        public static IdentityTerm Identity(string identity) =>
            new IdentityTerm(identity, UnspecifiedTerm.Instance);

        public static UnspecifiedTerm Unspecified() =>
            UnspecifiedTerm.Instance;

        public static BooleanTerm True() =>
            BooleanTerm.True;
        public static BooleanTerm False() =>
            BooleanTerm.False;

        public static BooleanTerm Constant(bool value) =>
            value ? BooleanTerm.True : BooleanTerm.False;

        public static Term Constant(object value) =>
            new ConstantTerm(value);

        public static ApplyTerm Apply(Term function, Term argument) =>
            new ApplyTerm(function, argument);

        public static LambdaTerm Lambda(string parameter, Term body) =>
            new LambdaTerm(parameter, body);

        public static AndTerm And(Term lhs, Term rhs) =>
            new AndTerm(lhs, rhs);

        public static Term If(Term condition, Term then, Term els) =>
            new IfTerm(condition, then, els);
    }

    ////////////////////////////////////////////////////////////

    public sealed class UnspecifiedTerm : Term
    {
        private UnspecifiedTerm()
        { }

        public override Term HigherOrder =>
            null!;

        public override Term Reduce(ReduceContext context) =>
            this;

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(InferContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is UnspecifiedTerm;

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
    }

    public sealed class IdentityTerm : Term
    {
        public new readonly string Identity;

        internal IdentityTerm(string identity, Term higherOrder)
        {
            this.Identity = identity;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        public override Term Reduce(ReduceContext context) =>
            context.GetBoundTerm(this.Identity) is Term term ?
                term.Reduce(context) :
                new IdentityTerm(this.Identity, this.HigherOrder.Reduce(context));

        public override Term Infer(InferContext context)
        {
            if (context.GetBoundTerm(this.Identity) is Term term)
            {
                return term.Infer(context);
            }

            var higherOrder = this.HigherOrder.Infer(context);
            if (higherOrder is UnspecifiedTerm)
            {
                return new IdentityTerm(this.Identity, context.CreatePlaceholder(UnspecifiedTerm.Instance));
            }
            else
            {
                return new IdentityTerm(this.Identity, higherOrder);
            }
        }

        public override Term Fixup(InferContext context) =>
            new IdentityTerm(this.Identity, this.HigherOrder.Fixup(context));

        public override bool Equals(Term? other) =>
            other is IdentityTerm rhs ? this.Identity.Equals(rhs.Identity) : false;
    }

    public sealed class PlaceholderTerm : Term
    {
        public readonly int Index;

        internal PlaceholderTerm(int index, Term higherOrder)
        {
            this.Index = index;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        public override Term Reduce(ReduceContext context) =>
            new PlaceholderTerm(this.Index, this.HigherOrder.Reduce(context));

        public override Term Infer(InferContext context) =>
            new PlaceholderTerm(this.Index, this.HigherOrder.Infer(context));

        public override Term Fixup(InferContext context) =>
            context.LookupUnifiedTerm(this);

        public override bool Equals(Term? other) =>
            other is PlaceholderTerm rhs ? this.Index.Equals(rhs.Index) : false;
    }

    public sealed class ConstantTerm : Term
    {
        public readonly object Value;

        internal ConstantTerm(object value) =>
            this.Value = value;

        public override Term HigherOrder =>
            Type(this.Value.GetType());

        public override Term Reduce(ReduceContext context) =>
            this;

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(InferContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is ConstantTerm rhs ? this.Value.Equals(rhs.Value) : false;
    }
}
