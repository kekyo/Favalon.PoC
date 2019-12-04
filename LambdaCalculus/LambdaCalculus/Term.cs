using LambdaCalculus.Operators;
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

        public override sealed bool Equals(object? other) =>
            this.Equals(other as Term);

        //////////////////////////////////

        public static UnspecifiedTerm Unspecified() =>
            UnspecifiedTerm.Instance;

        public static BooleanTerm True() =>
            BooleanTerm.True;
        public static BooleanTerm False() =>
            BooleanTerm.False;

        public static ClrTypeTerm Type(Type type) =>
            ClrTypeTerm.From(type);
        public static ClrTypeTerm Type<T>() =>
            ClrTypeTerm.From(typeof(T));

        public static BooleanTerm Constant(bool value) =>
            value ? BooleanTerm.True : BooleanTerm.False;
        public static ClrTypeTerm Constant(Type type) =>
            ClrTypeTerm.From(type);
        public static Term Constant(object value) =>
            value switch
            {
                bool boolValue => boolValue ? BooleanTerm.True : BooleanTerm.False,
                Type typeValue => ClrTypeTerm.From(typeValue),
                _ => new ConstantTerm(value)
            };

        public static IdentityTerm Identity(string identity) =>
            new IdentityTerm(identity, UnspecifiedTerm.Instance);

        public static ApplyTerm Apply(Term function, Term argument) =>
            new ApplyTerm(function, argument, UnspecifiedTerm.Instance);

        public static LambdaTerm Lambda(string parameter, Term body) =>
            new LambdaTerm(new IdentityTerm(parameter, UnspecifiedTerm.Instance), body);
        public static LambdaTerm Lambda(Term parameter, Term body) =>
            new LambdaTerm(parameter, body);

        public static AndAlsoTerm AndAlso(Term lhs, Term rhs) =>
            new AndAlsoTerm(lhs, rhs);

        public static EqualTerm Equal(Term lhs, Term rhs) =>
            new EqualTerm(lhs, rhs);

        public static IfTerm If(Term condition, Term then, Term @else) =>
            new IfTerm(condition, then, @else, UnspecifiedTerm.Instance);
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
            context.CreatePlaceholder(Instance);

        public override Term Fixup(InferContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is UnspecifiedTerm;

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
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
