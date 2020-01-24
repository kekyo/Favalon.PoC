using Favalon.Terms;
using Favalon.Terms.Logical;
using Favalon.Terms.Methods;
using Favalon.Terms.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon
{
    public sealed class ClrTermFactory : TermFactory
    {
        private ClrTermFactory()
        { }

        ///////////////////////////////////////////////////////////////////////////
        // Constants

        public static new BooleanTerm True() =>
            ClrConstantTerm.True;
        public static new BooleanTerm False() =>
            ClrConstantTerm.False;

        public static Term Constant(Type type) =>
            ClrTypeTerm.From(type);
        public static Term Constant(MethodInfo method) =>
            ClrMethodTerm.From(method);
        public static BooleanTerm Constant(bool value) =>
            ClrConstantTerm.From(value);
        public static Term Constant(object value) =>
            ClrConstantTerm.From(value);

        ///////////////////////////////////////////////////////////////////////////
        // Types

        public static Term Type<T>() =>
            ClrConstantTerm.From(typeof(T));

        public static ClrTypeSumTerm SumType(Term lhs, Term rhs) =>
            ClrTypeSumTerm.Create(lhs, rhs);
        public static Term? SumType(params Term[] terms) =>
            SumType((IEnumerable<Term>)terms);
        public static Term? SumType(IEnumerable<Term> terms) =>
            terms.ToArray() switch
            {
                Term[] ts when ts.Length == 1 => ts[0],
                Term[] ts when ts.Length >= 2 => ts.Aggregate(SumType),
                _ => null
            };

        public static ClrTypeProductTerm ProductType(Term lhs, Term rhs) =>
            ClrTypeProductTerm.Create(lhs, rhs);
        public static Term? ProductType(params Term[] terms) =>
            ProductType((IEnumerable<Term>)terms);
        public static Term? ProductType(IEnumerable<Term> terms) =>
            terms.ToArray() switch
            {
                Term[] ts when ts.Length == 1 => ts[0],
                Term[] ts when ts.Length >= 2 => ts.Aggregate(ProductType),
                _ => null
            };

        ///////////////////////////////////////////////////////////////////////////
        // Methods

        public static Term Method<T>(string name, params Type[] argumentTypes) =>
            ClrMethodTerm.From(typeof(T).GetMethod(name, argumentTypes));

        public static ClrMethodSumTerm SumMethod(Term lhs, Term rhs) =>
            ClrMethodSumTerm.Create(lhs, rhs);
        public static Term? SumMethod(params Term[] terms) =>
            SumMethod((IEnumerable<Term>)terms);
        public static Term? SumMethod(IEnumerable<Term> terms) =>
            terms.ToArray() switch
            {
                Term[] ts when ts.Length == 1 => ts[0],
                Term[] ts when ts.Length >= 2 => ts.Aggregate(SumMethod),
                _ => null
            };
    }
}
