namespace BasicSyntaxTree.Types
{
    public abstract class Type : System.IEquatable<Type>
    {
        protected Type() { }

        public abstract bool IsResolved { get; }

        public abstract bool Equals(Type other);

        // =======================================================================

        public static Type RuntimeType<T>() =>
            RuntimeType(typeof(T));

        public static Type RuntimeType(System.Type type) =>
            type.IsGenericTypeDefinition ? (Type)new TypeConstructorType(type) : new RuntimeType(type);

        public static KindType KindType<T>() =>
            KindType(typeof(T));

        public static KindType KindType(System.Type type) =>
            type.IsGenericTypeDefinition ? (KindType)new TypeConstructorType(type) : new RuntimeKindType(type);

        public static FunctionType Function(Type parameterType, Type resultType) =>
            new FunctionType(parameterType, resultType);
    }
}
