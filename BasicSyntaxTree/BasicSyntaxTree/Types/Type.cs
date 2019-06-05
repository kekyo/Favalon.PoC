namespace BasicSyntaxTree.Types
{
    public abstract class Type : System.IEquatable<Type>
    {
        protected Type() { }

        public abstract bool Equals(Type other);

        public static IntegerType Integer() =>
            new IntegerType();

        public static FunctionType Function(Type parameterType, Type resultType) =>
            new FunctionType(parameterType, resultType);

        public static UntypedType Untyped(int index) =>
            new UntypedType(index);
    }
}
