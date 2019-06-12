using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BasicSyntaxTree
{
    internal static class Utilities
    {
        public static IEnumerable<U> Collect<T, U>(this IEnumerable<T> enumerable, Func<T, U?> extractor)
            where U : class
        {
            foreach (var value in enumerable)
            {
                if (extractor(value) is U r)
                {
                    yield return r;
                }
            }
        }

        public static List<T> List<T>(params T[] values) =>
            new List<T>(values);

        public static Lazy<T> Lazy<T>(Func<T> factory) =>
            new Lazy<T>(factory);

        public static Dictionary<T, U> ToDictionary<T, U>(this IEnumerable<KeyValuePair<T, U>> dict)
        {
            var temp = (dict is ICollection coll) ? new Dictionary<T, U>(coll.Count) : new Dictionary<T, U>();
            foreach (var entry in dict)
            {
                temp.Add(entry.Key, entry.Value);
            }
            return temp;
        }

        public static Dictionary<T, U> ToDictionary<T, U>(this IEnumerable<(T key, U value)> dict)
        {
            var temp = (dict is ICollection coll) ? new Dictionary<T, U>(coll.Count) : new Dictionary<T, U>();
            foreach (var entry in dict)
            {
                temp.Add(entry.key, entry.value);
            }
            return temp;
        }

        private static readonly Dictionary<Type, string> knownTypeNames = new Dictionary<Type, string>
        {
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(bool), "bool" },
            { typeof(char), "char" },
            { typeof(string), "string" },
            { typeof(void), "void" },
            { typeof(Unit), "unit" },
            { typeof(IEnumerable<>), "seq" },
        };

        public static string PrettyPrint(this Type type)
        {
            if (type.IsGenericParameter)
            {
                return type.Name;
            }

            string name;
            if (type.IsGenericType)
            {
                var args = type.IsGenericTypeDefinition ?
                    string.Join(",", type.GetGenericArguments().Select(_ => string.Empty)) :
                    string.Join(",", type.GetGenericArguments().Select(PrettyPrint));

                var gtd = type.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition();
                if (!knownTypeNames.TryGetValue(gtd, out name))
                {
                    name = gtd.FullName.Substring(0, gtd.FullName.IndexOf('`'));
                }

                return $"{name}<{args}>";
            }

            if (knownTypeNames.TryGetValue(type, out name))
            {
                return name;
            }

            if (type.IsArray)
            {
                name = type.GetElementType().PrettyPrint();
                return $"array<{name}>";
            }

            return type.FullName;
        }

        public static string PrettyPrint(object obj)
        {
            if (obj == null)
            {
                return "(null)";
            }

            if (obj is string str)
            {
                return $"\"{str}\"";
            }
            else
            {
                return obj.ToString();
            }
        }
    }
}
