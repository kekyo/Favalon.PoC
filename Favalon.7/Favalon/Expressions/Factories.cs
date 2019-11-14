using System.Collections.Generic;
using System.Reflection;

namespace Favalon.Expressions
{
    public static class Factories
    {
        private static readonly Dictionary<System.Type, Type> types =
            new Dictionary<System.Type, Type>
            {
                { typeof(TypeInfo), Type.TypeType }
            };

        internal static Type FromType(System.Type type)
        {
            if (!types.TryGetValue(type, out var value))
            {
                value = new Type(type.GetTypeInfo());
                types.Add(type, value);
            }
            return value;
        }

        internal static Type FromType(TypeInfo type) =>
            FromType(type.AsType());

        internal static Type FromType<T>() =>
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
                System.Type type => FromType(type),
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
