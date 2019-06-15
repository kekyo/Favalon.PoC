namespace BasicSyntaxTree.Types
{
    public sealed class KindFunctionType : KindType, IRuntimeType
    {
        public readonly KindType ParameterType;
        public readonly KindType ResultType;

        internal KindFunctionType(KindType parameterType, KindType resultType)
        {
            this.ParameterType = parameterType;
            this.ResultType = resultType;
        }

        System.Type IRuntimeType.Type =>
            (this.ParameterType is IRuntimeType pt) && (this.ResultType is IRuntimeType rt) ?
                typeof(System.Func<,>).MakeGenericType(pt.Type, rt.Type) :
                throw new System.InvalidOperationException();

        public override bool IsResolved =>
            this.ParameterType.IsResolved && this.ResultType.IsResolved;

        public override bool Equals(Type other) =>
            other is FunctionType rhs ?
                (this.ParameterType.Equals(rhs.ParameterType) && this.ResultType.Equals(rhs.ResultType)) :
                false;

        public override string ToString()
        {
            if (this.ParameterType is KindFunctionType)
            {
                return $"({this.ParameterType}) -> {this.ResultType}";
            }
            else
            {
                return $"{this.ParameterType} -> {this.ResultType}";
            }
        }
    }
}
