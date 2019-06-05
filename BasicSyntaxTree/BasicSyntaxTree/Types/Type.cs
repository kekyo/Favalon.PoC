namespace BasicSyntaxTree.Types
{
    public abstract class Type : System.IEquatable<Type>
    {
        protected Type() { }

        public abstract bool Equals(Type other);

        public static ClsType<T> ClsType<T>() =>
            new ClsType<T>();

        public static ClsType ClsType(System.Type type) =>
            new ClsType(type);

        public static FunctionType Function(Type parameterType, Type resultType) =>
            new FunctionType(parameterType, resultType);

        public static UntypedType Untyped(int index) =>
            new UntypedType(index);
    }
}
