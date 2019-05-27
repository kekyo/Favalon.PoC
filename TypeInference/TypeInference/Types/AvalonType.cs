using System;

namespace TypeInference.Types
{
    public abstract class AvalonType : IEquatable<AvalonType>
    {
        protected AvalonType()
        {
        }

        public abstract bool Equals(AvalonType other);

        public static CLRType Create(Type type) =>
            new CLRType(type);
        public static CLRType Create<T>() =>
            new CLRType(typeof(T));
    }
}
