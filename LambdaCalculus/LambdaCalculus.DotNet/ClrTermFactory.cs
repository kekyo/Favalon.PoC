﻿using Favalon.Terms;
using Favalon.Terms.Logical;
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

        public static new BooleanTerm True() =>
            ClrConstantTerm.True;
        public static new BooleanTerm False() =>
            ClrConstantTerm.False;

        public static Term Type<T>() =>
            ClrConstantTerm.From(typeof(T));

        public static Term Constant(Type type) =>
            ClrConstantTerm.From(type);
        public static BooleanTerm Constant(bool value) =>
            ClrConstantTerm.From(value);
        public static Term Constant(object value) =>
            ClrConstantTerm.From(value);

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

        public static Term Method(MethodInfo method) =>
            ClrMethodTerm.From(new[] { method });
        public static Term Method(MethodInfo method0, params MethodInfo[] methods) =>
            ClrMethodTerm.From(new[] { method0 }.Concat(methods));
        public static Term Method(IEnumerable<MethodInfo> methods)
        {
            var ms = methods.ToArray();
            return ms.Length switch
            {
                0 => throw new ArgumentException(),
                _ => ClrMethodTerm.From(ms)
            };
        }

    }
}
