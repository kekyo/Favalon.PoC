using Favalon.Terms;
using Favalon.Terms.Algebric;
using Favalon.Terms.Logical;
using Favalon.Terms.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon
{
    partial class Term
    {
        public static UnspecifiedTerm Unspecified() =>
            UnspecifiedTerm.Instance;
        public static LambdaTerm UnspecifiedFunction() =>
            LambdaTerm.Unspecified;
        public static KindTerm Kind() =>
            KindTerm.Instance;
        public static LambdaTerm KindFunction() =>
            LambdaTerm.Kind;

        public static BooleanTerm True() =>
            BooleanTerm.True;
        public static BooleanTerm False() =>
            BooleanTerm.False;

        public static DeclareTypeTerm Type(Term declare) =>
            TypeTerm.From(declare, UnspecifiedTerm.Instance);

        public static Term Type(Type type) =>
            TypeTerm.From(type);
        public static Term Type<T>() =>
            TypeTerm.From(typeof(T));

        public static MethodTerm Method(MethodInfo method) =>
            MethodTerm.From(new[] { method });
        public static MethodTerm Method(MethodInfo method0, params MethodInfo[] methods) =>
            MethodTerm.From(new[] { method0 }.Concat(methods));
        public static MethodTerm Method(IEnumerable<MethodInfo> methods)
        {
            var ms = methods.ToArray();
            return ms.Length switch
            {
                0 => throw new ArgumentException(),
                _ => MethodTerm.From(ms)
            };
        }

        public static Term Constant(object value) =>
            value switch
            {
                bool boolValue => BooleanTerm.From(boolValue),
                Type typeValue => TypeTerm.From(typeValue),
                _ => new ConstantTerm(value)
            };

        public static IdentityTerm Identity(string identity) =>
            new IdentityTerm(identity, UnspecifiedTerm.Instance);
        public static IdentityTerm Identity(string identity, Term higherOrder) =>
            new IdentityTerm(identity, higherOrder);

        public static ApplyTerm Apply(Term function, Term argument) =>
            new ApplyTerm(function, argument, UnspecifiedTerm.Instance);
        public static ApplyTerm Apply(Term function, Term argument, Term higherOrder) =>
            new ApplyTerm(function, argument, higherOrder);

        public static LambdaTerm Lambda(string parameter, Term body) =>
            LambdaTerm.Create(new IdentityTerm(parameter, UnspecifiedTerm.Instance), body);
        public static LambdaTerm Lambda(Term parameter, Term body) =>
            LambdaTerm.Create(parameter, body);

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

        public static PairTerm Pair(Term lhs, Term rhs) =>
            new PairTerm(lhs, rhs);

        public static ProductTerm Product(Term term0, params Term[] terms) =>
            new ProductTerm(new[] { term0 }.Concat(terms).ToArray());

        public static SumTerm Sum(Term term0, params Term[] terms) =>
            new SumTerm(new[] { term0 }.Concat(terms).ToArray());
        public static SumTerm Sum(IEnumerable<Term> terms)
        {
            var ts = terms.ToArray();
            return ts.Length switch
            {
                0 => throw new ArgumentException(),
                _ => new SumTerm(ts)
            };
        }
        public static Term? ComposedSum(IEnumerable<Term> terms)
        {
            var ts = terms.ToArray();
            return ts.Length switch
            {
                0 => null,
                1 => ts[0],
                _ => new SumTerm(ts)
            };
        }

        public static MatchTerm Match(PairTerm matcher0, params PairTerm[] matchers) =>
            new MatchTerm(new[] { matcher0 }.Concat(matchers).ToArray(), UnspecifiedTerm.Instance);

        public static DiscriminatedUnionTypeTerm DiscriminatedUnionType(
            params BindExpressionTerm[] constructors) =>
            new DiscriminatedUnionTypeTerm(constructors, UnspecifiedTerm.Instance);
    }
}
