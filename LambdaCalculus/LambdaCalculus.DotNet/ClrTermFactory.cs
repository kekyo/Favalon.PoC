using Favalon.Terms;
using Favalon.Terms.Logical;
using System;

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
    }
}
