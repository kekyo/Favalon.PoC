namespace BasicSyntaxTree.Typed.Types
{
    public sealed class HigherOrderType : TypedType
    {
        public readonly Type TypeConstructor;
        public readonly Type TypeArgument;

        internal HigherOrderType(Type typeConstructor, Type typeArgument)
        {
            this.TypeConstructor = typeConstructor;
            this.TypeArgument = typeArgument;
        }

        public override bool IsResolved =>
            this.TypeConstructor.IsResolved && this.TypeArgument.IsResolved;

        public override bool Equals(Type other) =>
            other is HigherOrderType rhs ?
            (this.TypeConstructor.Equals(rhs.TypeConstructor) && this.TypeArgument.Equals(rhs.TypeArgument)) :
            false;

        public override string ToString() =>
            $"{this.TypeConstructor}<{this.TypeArgument}>";
    }
}
