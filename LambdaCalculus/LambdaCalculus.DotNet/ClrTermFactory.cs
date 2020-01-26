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
        // CLR Constants

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
        // CLR Types

        public static Term ClrType<T>() =>
            ClrConstantTerm.From(typeof(T));

        public static ClrTypeSumTerm SumClrType(Term lhs, Term rhs) =>
            ClrTypeSumTerm.Create(lhs, rhs);
        public static Term? SumClrType(params Term[] terms) =>
            SumClrType((IEnumerable<Term>)terms);
        public static Term? SumClrType(IEnumerable<Term> terms) =>
            terms.ToArray() switch
            {
                Term[] ts when ts.Length == 1 => ts[0],
                Term[] ts when ts.Length >= 2 => ts.Aggregate(SumClrType),
                _ => null
            };

        public static ClrTypeProductTerm ProductClrType(Term lhs, Term rhs) =>
            ClrTypeProductTerm.Create(lhs, rhs);
        public static Term? ProductClrType(params Term[] terms) =>
            ProductClrType((IEnumerable<Term>)terms);
        public static Term? ProductClrType(IEnumerable<Term> terms) =>
            terms.ToArray() switch
            {
                Term[] ts when ts.Length == 1 => ts[0],
                Term[] ts when ts.Length >= 2 => ts.Aggregate(ProductClrType),
                _ => null
            };

        ///////////////////////////////////////////////////////////////////////////
        // CLR Methods

        public static Term ClrMethod<T>(string name, params Type[] argumentTypes) =>
            ClrMethodTerm.From(typeof(T).GetMethod(name, argumentTypes));

        public static ClrMethodSumTerm SumClrMethod(Term lhs, Term rhs) =>
            ClrMethodSumTerm.Create(lhs, rhs);
        public static Term? SumClrMethod(params Term[] terms) =>
            SumClrMethod((IEnumerable<Term>)terms);
        public static Term? SumClrMethod(IEnumerable<Term> terms) =>
            terms.ToArray() switch
            {
                Term[] ts when ts.Length == 1 => ts[0],
                Term[] ts when ts.Length >= 2 => ts.Aggregate(SumClrMethod),
                _ => null
            };
    }
}
