namespace BasicSyntaxTree.Types.Unresolved
{
    public sealed class UnresolvedFunctionType : UnresolvedType
    {
        public readonly UnresolvedType ParameterType;
        public readonly UnresolvedType ExpressionType;

        internal UnresolvedFunctionType(UnresolvedType parameterType, UnresolvedType expressionType)
        {
            this.ParameterType = parameterType;
            this.ExpressionType = expressionType;
        }

        public override bool IsResolved =>
            this.ParameterType.IsResolved && this.ExpressionType.IsResolved;

        public override bool Equals(Type other) =>
            other is UnresolvedFunctionType rhs ?
                (this.ParameterType.Equals(rhs.ParameterType) && this.ExpressionType.Equals(rhs.ExpressionType)) :
                false;

        public override string ToString()
        {
            if (this.ParameterType is UnresolvedFunctionType)
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
