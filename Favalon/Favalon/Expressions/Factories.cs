using System;
using System.Collections.Generic;
using System.Reflection;

namespace Favalon.Expressions
{
    public static class Factories
    {
        private static readonly Dictionary<Type, Instance<TypeInfo>> types =
            new Dictionary<Type, Instance<TypeInfo>>();

        internal static Instance<TypeInfo> FromType(Type type)
        {
            if (!types.TryGetValue(type, out var value))
            {
                value = new Instance<TypeInfo>(type.GetTypeInfo());
                types.Add(type, value);
            }
            return value;
        }

        internal static Instance<TypeInfo> FromType(TypeInfo type) =>
            FromType(type.GetTypeInfo());

        internal static Instance<TypeInfo> FromType<T>() =>
            FromType(typeof(T).GetTypeInfo());

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

        public static Instance<string> Value(string stringValue) =>
            new Instance<string>(stringValue);

        public static Instance<int> Value(int intValue) =>
            new Instance<int>(intValue);

        public static Instance<double> Value(double doubleValue) =>
            new Instance<double>(doubleValue);

        public static Instance<T> Value<T>(T value) =>
            new Instance<T>(value);
    }
}
