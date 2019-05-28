using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TypeInferences.Types
{
    public abstract partial class AvalonType
    {
        public static AvalonType Create(Type type) =>
            Create(type.GetTypeInfo());
        public static AvalonType Create<T>() =>
            Create(typeof(T).GetTypeInfo());

        public static AvalonType Create(TypeInfo typeInfo)
        {
            if (typeInfo == typeof(int).GetTypeInfo())
            {
                return Int32Type.Instance;
            }
            if (typeInfo == typeof(double).GetTypeInfo())
            {
                return DoubleType.Instance;
            }
            if (typeInfo == typeof(string).GetTypeInfo())
            {
                return StringType.Instance;
            }

            throw new ArgumentException();
        }
    }
}
