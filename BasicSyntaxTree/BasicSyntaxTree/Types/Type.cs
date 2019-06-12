using BasicSyntaxTree.Types.Unresolved;

namespace BasicSyntaxTree.Types
{
    public abstract class Type : System.IEquatable<Type>
    {
        protected Type() { }

        public abstract bool IsResolved { get; }

        public abstract bool Equals(Type other);

        // =======================================================================

        public static UnresolvedType ClrType<T>() =>
            ClrType(typeof(T));

        public static UnresolvedType ClrType(System.Type type)
        {
            if (type.IsGenericType)
            {
                // .NET CLS special case: List<> ===> List<>
                if (type.IsGenericTypeDefinition)
                {
                    return new UnresolvedTypeConstructor(type);
                }
                // .NET CLS special case: List<int> ===> List<> -> int
                else
                {
                    // TODO: nested type
                    return Function(
                        ClrType(type.GetGenericTypeDefinition()),
                        ClrType(type.GetGenericArguments()[0]));
                }
            }
            // int, List<>
            else
            {
                return new UnresolvedClrType(type);
            }
        }

        public static UnresolvedFunctionType Function(UnresolvedType parameterType, UnresolvedType resultType) =>
            new UnresolvedFunctionType(parameterType, resultType);
    }
}
