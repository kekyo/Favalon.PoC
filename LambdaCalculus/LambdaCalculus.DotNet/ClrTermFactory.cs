using Favalon.Terms;
using Favalon.Terms.Logical;
using Favalon.Terms.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalon
{
    public sealed class ClrTermFactory : TermFactory
    {
        private ClrTermFactory()
        { }

        //public static MethodTerm Method(MethodInfo method) =>
        //    MethodTerm.From(new[] { method });
        //public static MethodTerm Method(MethodInfo method0, params MethodInfo[] methods) =>
        //    MethodTerm.From(new[] { method0 }.Concat(methods));
        //public static MethodTerm Method(IEnumerable<MethodInfo> methods)
        //{
        //    var ms = methods.ToArray();
        //    return ms.Length switch
        //    {
        //        0 => throw new ArgumentException(),
        //        _ => MethodTerm.From(ms)
        //    };
        //}

        public static Term Type<T>() =>
            ConstantTerm.From(typeof(T));

        public static Term Constant(Type type) =>
            ConstantTerm.From(type);
        public static BooleanTerm Constant(bool value) =>
            ConstantTerm.From(value);
        public static Term Constant(object value) =>
            ConstantTerm.From(value);

        public static Term SumType(Term lhs, Term rhs) =>
            ClrTypeSumTerm.Create(lhs, rhs);
        public static Term SumType(params Term[] terms) =>
            SumType((IEnumerable<Term>)terms);
        public static Term SumType(IEnumerable<Term> terms) =>
            terms.ToArray() switch
            {
                Term[] ts when ts.Length == 1 => ts[0],
                Term[] ts when ts.Length >= 2 => ts.Aggregate(SumType),
                _ => throw new InvalidOperationException()
            };
    }
}
