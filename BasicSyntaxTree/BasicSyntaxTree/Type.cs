using BasicSyntaxTree.Untyped.Types;

namespace BasicSyntaxTree
{
    public abstract class Type : System.IEquatable<Type>
    {
        protected Type() { }

        public abstract bool IsResolved { get; }

        public abstract bool Equals(Type other);

        // =======================================================================

        public static UntypedClsType<T> ClsType<T>() =>
            new UntypedClsType<T>();

        public static UntypedClsType ClsType(System.Type type) =>
            new UntypedClsType(type);

        public static UntypedFunctionType Function(UntypedType parameterType, UntypedType resultType) =>
            new UntypedFunctionType(parameterType, resultType);

        public static UnspecifiedType Unspecified(int index) =>
            new UnspecifiedType(index);
    }
}
