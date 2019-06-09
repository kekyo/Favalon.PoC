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

        public static UntypedClrType<T> ClrType<T>() =>
            UntypedClrType<T>.Instance;

        public static UntypedClrType ClrType(System.Type type) =>
            new UntypedClrType(type);

        public static UntypedFunctionType Function(UntypedType parameterType, UntypedType resultType) =>
            new UntypedFunctionType(parameterType, resultType);
    }
}
