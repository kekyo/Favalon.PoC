using Favalet.Expressions;
using System;
using System.Reflection;

namespace Favalet
{
    public static class CLRGenerator
    {
        public static TypeTerm Type<T>() =>
            TypeTerm.From(typeof(T));
        public static TypeTerm Type(Type runtimeType) =>
            TypeTerm.From(runtimeType);

        public static MethodTerm Method(MethodBase runtimeMethod) =>
            MethodTerm.From(runtimeMethod);

        public static ConstantTerm Constant(object value) =>
            ConstantTerm.Create(value);
    }
}
