namespace BasicSyntaxTree.Untyped.Types
{
    public sealed class UntypedHigherOrderType : UntypedType
    {
        public readonly UntypedType TypeConstructor;
        public readonly UntypedType TypeArgument;

        internal UntypedHigherOrderType(UntypedType typeConstructor, UntypedType typeArgument)
        {
            this.TypeConstructor = typeConstructor;
            this.TypeArgument = typeArgument;
        }

        public override bool IsResolved =>
            this.TypeConstructor.IsResolved && this.TypeArgument.IsResolved;

        public override bool Equals(Type other) =>
            other is UntypedHigherOrderType rhs ?
            (this.TypeConstructor.Equals(rhs.TypeConstructor) && this.TypeArgument.Equals(rhs.TypeArgument)) :
            false;

        public override string ToString() =>
            $"{this.TypeConstructor}<{this.TypeArgument}>";
    }
}
