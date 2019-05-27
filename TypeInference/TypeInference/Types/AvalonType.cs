using System;
using System.Reflection;

namespace TypeInference.Types
{
    public enum ComparedResult
    {
        Invalid,
        Equal,
        NarrowThan,
        WideThan
    }

    public abstract class AvalonType : IEquatable<AvalonType>
    {
        protected AvalonType()
        {
        }

        public abstract bool Equals(AvalonType rhs);

        public abstract ComparedResult CompareTo(AvalonType rhs);

        public static CLRType Create(Type type) =>
            new CLRType(type.GetTypeInfo());
        public static CLRType Create(TypeInfo typeInfo) =>
            new CLRType(typeInfo);
        public static CLRType Create<T>() =>
            new CLRType(typeof(T).GetTypeInfo());
    }
}
