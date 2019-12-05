using LambdaCalculus.Algebric;
using LambdaCalculus.Operators;
using System;

#pragma warning disable 659

namespace LambdaCalculus
{
    public abstract class Term : IEquatable<Term?>
    {
        public abstract Term HigherOrder { get; }

        public abstract Term Reduce(ReduceContext context);

        public abstract Term Infer(InferContext context);

        public abstract Term Fixup(FixupContext context);

        public abstract bool Equals(Term? other);

        bool IEquatable<Term?>.Equals(Term? other) =>
            this.Equals(other);

        public override sealed bool Equals(object? other) =>
            this.Equals(other as Term);

        public void Deconstruct(out Term higherOrder) =>
            higherOrder = this.HigherOrder;

        //////////////////////////////////

        public static UnspecifiedTerm Unspecified() =>
            UnspecifiedTerm.Instance;
        public static LambdaTerm UnspecifiedFunction() =>
            LambdaTerm.Unspecified;

        public static BooleanTerm True() =>
            BooleanTerm.True;
        public static BooleanTerm False() =>
            BooleanTerm.False;

        public static TypeTerm Type(Type type) =>
            TypeTerm.From(type);
        public static ClrTypeTerm Type<T>() =>
            (ClrTypeTerm)TypeTerm.From(typeof(T));

        public static BooleanTerm Constant(bool value) =>
            BooleanTerm.From(value);
        public static TypeTerm Constant(Type type) =>
            TypeTerm.From(type);
        public static Term Constant(object value) =>
            value switch
            {
                bool boolValue => BooleanTerm.From(boolValue),
                Type typeValue => TypeTerm.From(typeValue),
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

        public static BindTerm Bind(string bound, Term body, Term continuation) =>
            new BindTerm(new IdentityTerm(bound, UnspecifiedTerm.Instance), body, continuation);
        public static BindTerm Bind(Term bound, Term body, Term continuation) =>
            new BindTerm(bound, body, continuation);

        public static NotTerm Not(Term term) =>
            new NotTerm(term);
        public static AndAlsoTerm AndAlso(Term lhs, Term rhs) =>
            new AndAlsoTerm(lhs, rhs);
        public static OrElseTerm OrElse(Term lhs, Term rhs) =>
            new OrElseTerm(lhs, rhs);

        public static EqualTerm Equal(Term lhs, Term rhs) =>
            new EqualTerm(lhs, rhs);

        public static IfTerm If(Term condition, Term then, Term @else) =>
            new IfTerm(condition, then, @else, LambdaCalculus.UnspecifiedTerm.Instance);

        public static ProductTerm Product(Term term0, Term term1) =>
            ProductTerm.Create(term0, term1);
        public static ProductTerm Product(Term term0, Term term1, params Term[] terms) =>
            ProductTerm.Create(term0, term1, terms);

        public static SumTerm Sum(Term term0, Term term1) =>
            SumTerm.Create(term0, term1);
        public static SumTerm Sum(Term term0, Term term1, params Term[] terms) =>
            SumTerm.Create(term0, term1, terms);
    }

    ////////////////////////////////////////////////////////////

    public sealed class UnspecifiedTerm : Term
    {
        private static readonly int hashCode =
           typeof(UnspecifiedTerm).GetHashCode();

        private UnspecifiedTerm()
        { }

        public override Term HigherOrder =>
            null!;

        public override Term Reduce(ReduceContext context) =>
            this;

        public override Term Infer(InferContext context) =>
            context.CreatePlaceholder(Instance);

        public override Term Fixup(FixupContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is UnspecifiedTerm;

        public override int GetHashCode() =>
            hashCode;

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
    }

    public sealed class PlaceholderTerm : Term
    {
        private static readonly int hashCode =
            typeof(PlaceholderTerm).GetHashCode();

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

        public override Term Fixup(FixupContext context) =>
            context.LookupUnifiedTerm(this);

        public override bool Equals(Term? other) =>
            other is PlaceholderTerm rhs ? this.Index.Equals(rhs.Index) : false;

        public override int GetHashCode() =>
            hashCode ^ this.Index;
    }

    public sealed class ConstantTerm : Term
    {
        private static readonly int hashCode =
            typeof(ConstantTerm).GetHashCode();

        public readonly object Value;

        internal ConstantTerm(object value) =>
            this.Value = value;

        public override Term HigherOrder =>
            Type(this.Value.GetType());

        public override Term Reduce(ReduceContext context) =>
            this;

        public override Term Infer(InferContext context) =>
            this;

        public override Term Fixup(FixupContext context) =>
            this;

        public override bool Equals(Term? other) =>
            other is ConstantTerm rhs ? this.Value.Equals(rhs.Value) : false;

        public override int GetHashCode() =>
            hashCode ^ this.Value.GetHashCode();
    }
}
