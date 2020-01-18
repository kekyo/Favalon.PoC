using Favalon.Terms;
using Favalon.Terms.Logical;
using Favalon.Terms.Types;
using System;

namespace Favalon
{
    public static class ClrTermFactory
    {
        public static Term Type<T>() =>
            ClrTypeTerm.From(typeof(T));

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

        public static Term Constant(Type type) =>
            ClrTypeTerm.From(type);
        public static BooleanTerm Constant(bool boolValue) =>
            BooleanTerm.From(boolValue);
        public static Term Constant(object value) =>
            value switch
            {
                // TODO: null
                Type type => ClrTypeTerm.From(type),
                bool boolValue => BooleanTerm.From(boolValue),
                _ => new ConstantTerm(value)
            };
    }
}
