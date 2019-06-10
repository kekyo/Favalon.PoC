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
            // .NET CLS special case: List<int> ===> List<> -> int
            if (type.IsGenericType && !type.IsGenericTypeDefinition)
            {
                return HigherOrder(
                    ClrType(type.GetGenericTypeDefinition()),
                    ClrType(type.GetGenericArguments()[0]));
            }
            // int, List<>
            else
            {
                return new UntypedClrType(type);
            }
        }

        public static UntypedFunctionType Function(UntypedType parameterType, UntypedType resultType) =>
            new UntypedFunctionType(parameterType, resultType);

        public static UntypedHigherOrderType HigherOrder(UntypedType typeConstructor, UntypedType typeArgument) =>
            new UntypedHigherOrderType(typeConstructor, typeArgument);
    }
}
