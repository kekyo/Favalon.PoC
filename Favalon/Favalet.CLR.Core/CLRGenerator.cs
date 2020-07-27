using Favalet.Expressions;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Favalet
{
    public static class CLRGenerator
    {
        [DebuggerHidden]
        public static ITerm Type<T>() =>
            TypeTerm.From(typeof(T));
        [DebuggerHidden]
        public static ITerm Type(Type runtimeType) =>
            TypeTerm.From(runtimeType);

        [DebuggerHidden]
        public static MethodTerm Method(MethodBase runtimeMethod) =>
            MethodTerm.From(runtimeMethod);

        [DebuggerHidden]
        public static ConstantTerm Constant(object value) =>
            ConstantTerm.From(value);
    }
}
