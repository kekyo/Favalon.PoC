using System;
using System.Collections.Generic;
using System.Reflection;

namespace Favalon.Expressions
{
    public static class Factories
    {
        private static readonly Instance typeInfo =
            new Instance(typeof(TypeInfo).GetTypeInfo(), null!);
        private static readonly Dictionary<Type, Instance> types =
            new Dictionary<Type, Instance>();

        static Factories() =>
            typeInfo.higherOrder = typeInfo;

        internal static Instance FromType(Type type)
        {
            if (!types.TryGetValue(type, out var value))
            {
                value = new Instance(type.GetTypeInfo(), typeInfo);
                types.Add(type, value);
            }
            return value;
        }

        internal static Instance FromType(TypeInfo type) =>
            FromType(type.AsType());

        internal static Instance FromType<T>() =>
            FromType(typeof(T));

        public static Unknown Unknown(Terms.Term term) =>
            new Unknown(term);

        public static Value Value(object value) =>
            value switch
            {
                null => Null.Instance,
                string stringValue => new String(stringValue),
                int intValue => new Number<int>(intValue),
                double doubleValue => new Number<double>(doubleValue),
                TypeInfo type => FromType(type),
                Type type => FromType(type),
                _ => new Instance(value, FromType(value.GetType()))
            };

        public static String Value(string stringValue) =>
            new String(stringValue);

        public static Number<int> Value(int intValue) =>
            new Number<int>(intValue);

        public static Number<double> Value(double doubleValue) =>
            new Number<double>(doubleValue);

        public static CallMethod CallMethod(MethodInfo method, Expression argument) =>
            new CallMethod(method, argument);

        public static RunExecutable RunExecutable(string path, Expression argument) =>
            new RunExecutable(path, argument);
    }
}
