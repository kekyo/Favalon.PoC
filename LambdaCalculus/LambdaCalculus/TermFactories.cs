using Favalon.Algebric;
using Favalon.Operators;
using System;

namespace Favalon
{
    partial class Term
    {
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

        public static BindExpressionTerm Bind(string bound, Term body) =>
            new BindExpressionTerm(new IdentityTerm(bound, UnspecifiedTerm.Instance), body);
        public static BindExpressionTerm Bind(Term bound, Term body) =>
            new BindExpressionTerm(bound, body);

        public static BindTerm Bind(string bound, Term body, Term continuation) =>
            new BindTerm(new BindExpressionTerm(new IdentityTerm(bound, UnspecifiedTerm.Instance), body), continuation);
        public static BindTerm Bind(Term bound, Term body, Term continuation) =>
            new BindTerm(new BindExpressionTerm(bound, body), continuation);

        public static NotTerm Not(Term term) =>
            new NotTerm(term);
        public static AndAlsoTerm AndAlso(Term lhs, Term rhs) =>
            new AndAlsoTerm(lhs, rhs);
        public static OrElseTerm OrElse(Term lhs, Term rhs) =>
            new OrElseTerm(lhs, rhs);

        public static EqualTerm Equal(Term lhs, Term rhs) =>
            new EqualTerm(lhs, rhs);

        public static IfTerm If(Term condition, Term then, Term @else) =>
            new IfTerm(condition, then, @else, UnspecifiedTerm.Instance);

        public static ProductTerm Product(Term term0, Term term1) =>
            ProductTerm.Create(term0, term1);
        public static ProductTerm Product(Term term0, Term term1, params Term[] terms) =>
            ProductTerm.Create(term0, term1, terms);

        public static SumTerm Sum(Term term0, Term term1) =>
            SumTerm.Create(term0, term1);
        public static SumTerm Sum(Term term0, Term term1, params Term[] terms) =>
            SumTerm.Create(term0, term1, terms);
    }
}
