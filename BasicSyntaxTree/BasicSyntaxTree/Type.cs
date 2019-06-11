using BasicSyntaxTree.Untyped;
using BasicSyntaxTree.Untyped.Types;

namespace BasicSyntaxTree
{
    public abstract class Type : System.IEquatable<Type>
    {
        protected Type() { }

        public abstract bool IsResolved { get; }

        public abstract bool Equals(Type other);

        // =======================================================================

        public static UntypedType ClrType<T>() =>
            ClrType(typeof(T));

        public static UntypedType ClrType(System.Type type)
        {
            if (type.IsGenericType)
            {
                // .NET CLS special case: List<> ===> List<>
                if (type.IsGenericTypeDefinition)
                {
                    return new UntypedTypeConstructor(type);
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
                return new UntypedClrType(type);
            }
        }

        public static UntypedFunctionType Function(UntypedType parameterType, UntypedType resultType) =>
            new UntypedFunctionType(parameterType, resultType);
    }
}
