namespace BasicSyntaxTree.Types
{
    public abstract class Type : System.IEquatable<Type>
    {
        protected Type() { }

        public abstract bool Equals(Type other);

        public static DotnetType Dotnet<T>() =>
            new DotnetType(typeof(T));

        public static DotnetType Dotnet(System.Type type) =>
            new DotnetType(type);

        public static FunctionType Function(Type parameterType, Type resultType) =>
            new FunctionType(parameterType, resultType);

        public static UntypedType Untyped(int index) =>
            new UntypedType(index);
    }
}
