using Favalon.Terms;
using Favalon.Terms.Logical;
using Favalon.Terms.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalon
{
    public abstract class TermFactory
    {
        protected TermFactory() =>
            throw new InvalidOperationException();

        ///////////////////////////////////////////////////////////////////////////
        // Lambda calculus

        public static UnspecifiedTerm Unspecified() =>
            UnspecifiedTerm.Instance;
        public static KindTerm Kind() =>
            KindTerm.Instance;
        public static KindTerm Kind(string identity) =>
            KindTerm.Create(identity);

        public static FreeVariableTerm Identity(string identity) =>
            FreeVariableTerm.Create(identity, UnspecifiedTerm.Instance);
        public static FreeVariableTerm Identity(string identity, Term higherOrder) =>
            FreeVariableTerm.Create(identity, higherOrder);

        public static ApplyTerm Apply(Term function, Term argument) =>
            ApplyTerm.Create(function, argument, UnspecifiedTerm.Instance);
        public static ApplyTerm Apply(Term function, Term argument, Term higherOrder) =>
            ApplyTerm.Create(function, argument, higherOrder);

        public static LambdaTerm Lambda(string parameter, Term body) =>
            (LambdaTerm)LambdaTerm.From(FreeVariableTerm.Create(parameter, UnspecifiedTerm.Instance), body);
        public static LambdaTerm Lambda(Term parameter, Term body) =>
            (LambdaTerm)LambdaTerm.From(parameter, body);

        ///////////////////////////////////////////////////////////////////////////
        // Bind

        public static BindExpressionTerm BindMutable(string bound, Term body) =>
            BindExpressionTerm.Create(BoundIdentityTerm.Create(bound, UnspecifiedTerm.Instance), body);
        public static BindExpressionTerm BindMutable(Term bound, Term body) =>
            BindExpressionTerm.Create(bound, body);

        public static BindTerm Bind(string bound, Term body, Term continuation) =>
            BindTerm.Create(BindExpressionTerm.Create(BoundIdentityTerm.Create(bound, UnspecifiedTerm.Instance), body), continuation);
        public static BindTerm Bind(Term bound, Term body, Term continuation) =>
            BindTerm.Create(BindExpressionTerm.Create(bound, body), continuation);

        ///////////////////////////////////////////////////////////////////////////
        // Algebraic

        public static Term Sum(Term lhs, Term rhs) =>
            ApplyTerm.Create(
                ApplyTerm.Create(
                    FreeVariableTerm.Create("+", LambdaTerm.Unspecified2),
                    lhs,
                    UnspecifiedTerm.Instance),
                rhs, UnspecifiedTerm.Instance);
        public static Term? Sum(params Term[] terms) =>
            Sum((IEnumerable<Term>)terms);
        public static Term? Sum(IEnumerable<Term> terms) =>
            terms.ToArray() switch
            {
                Term[] ts when ts.Length == 1 => ts[0],
                Term[] ts when ts.Length >= 2 => ts.Aggregate(Sum),
                _ => null
            };

        public static Term Product(Term lhs, Term rhs) =>
            ApplyTerm.Create(
                ApplyTerm.Create(
                    FreeVariableTerm.Create("*", LambdaTerm.Unspecified2),
                    lhs,
                    UnspecifiedTerm.Instance),
                rhs, UnspecifiedTerm.Instance);
        public static Term? Product(params Term[] terms) =>
            Product((IEnumerable<Term>)terms);
        public static Term? Product(IEnumerable<Term> terms) =>
            terms.ToArray() switch
            {
                Term[] ts when ts.Length == 1 => ts[0],
                Term[] ts when ts.Length >= 2 => ts.Aggregate(Product),
                _ => null
            };

        ///////////////////////////////////////////////////////////////////////////
        // Logical

        internal static BooleanTerm True(Term higherOrder) =>
            BooleanTerm.From(true, higherOrder);
        internal static BooleanTerm False(Term higherOrder) =>
            BooleanTerm.From(false, higherOrder);

        public static BooleanTerm True() =>
            BooleanTerm.True;
        public static BooleanTerm False() =>
            BooleanTerm.False;

        public static AndAlsoTerm AndAlso(Term lhs, Term rhs) =>
            AndAlsoTerm.Create(lhs, rhs, BooleanTerm.Type);
        public static OrElseTerm OrElse(Term lhs, Term rhs) =>
            OrElseTerm.Create(lhs, rhs, BooleanTerm.Type);
        public static NotTerm Not(Term argument) =>
            NotTerm.Create(argument, BooleanTerm.Type);

        ///////////////////////////////////////////////////////////////////////////
        // Types

        public static TypeSumTerm SumType(Term lhs, Term rhs) =>
            TypeSumTerm.Create(lhs, rhs, UnspecifiedTerm.Instance);
        public static Term? SumType(params Term[] terms) =>
            SumType((IEnumerable<Term>)terms);
        public static Term? SumType(IEnumerable<Term> terms) =>
            terms.ToArray() switch
            {
                Term[] ts when ts.Length == 1 => ts[0],
                Term[] ts when ts.Length >= 2 => ts.Aggregate(SumType),
                _ => null
            };

        public static TypeProductTerm ProductType(Term lhs, Term rhs) =>
            TypeProductTerm.Create(lhs, rhs, UnspecifiedTerm.Instance);
        public static Term? ProductType(params Term[] terms) =>
            ProductType((IEnumerable<Term>)terms);
        public static Term? ProductType(IEnumerable<Term> terms) =>
            terms.ToArray() switch
            {
                Term[] ts when ts.Length == 1 => ts[0],
                Term[] ts when ts.Length >= 2 => ts.Aggregate(ProductType),
                _ => null
            };
    }
}
