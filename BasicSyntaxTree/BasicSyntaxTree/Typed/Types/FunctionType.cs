namespace BasicSyntaxTree.Typed.Types
{
    public sealed class FunctionType : TypedType
    {
        public readonly Type ParameterType;
        public readonly Type ExpressionType;

        internal FunctionType(Type parameterType, Type expressionType)
        {
            this.ParameterType = parameterType;
            this.ExpressionType = expressionType;
        }

        public override bool IsResolved =>
            this.ParameterType.IsResolved && this.ExpressionType.IsResolved;

        public override bool Equals(Type other) =>
            other is FunctionType rhs ?
                (this.ParameterType.Equals(rhs.ParameterType) && this.ExpressionType.Equals(rhs.ExpressionType)) :
                false;

        public override string ToString()
        {
            if (this.ParameterType is FunctionType)
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
