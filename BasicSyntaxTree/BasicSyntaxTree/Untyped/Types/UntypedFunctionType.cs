namespace BasicSyntaxTree.Untyped.Types
{
    public sealed class UntypedFunctionType : UntypedType
    {
        public readonly UntypedType ParameterType;
        public readonly UntypedType ExpressionType;

        internal UntypedFunctionType(UntypedType parameterType, UntypedType expressionType)
        {
            this.ParameterType = parameterType;
            this.ExpressionType = expressionType;
        }

        public override bool IsResolved =>
            this.ParameterType.IsResolved && this.ExpressionType.IsResolved;

        public override bool Equals(Type other) =>
            other is UntypedFunctionType rhs ?
                (this.ParameterType.Equals(rhs.ParameterType) && this.ExpressionType.Equals(rhs.ExpressionType)) :
                false;

        public override string ToString()
        {
            if (this.ParameterType is UntypedFunctionType)
            {
                return $"({this.ParameterType}) -> {this.ExpressionType}";
            }
            else
            {
                return $"{this.ParameterType} -> {this.ExpressionType}";
            }
        }
    }
}
