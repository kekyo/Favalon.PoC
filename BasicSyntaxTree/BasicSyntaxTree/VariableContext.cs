using System.Collections.Generic;

namespace BasicSyntaxTree
{
    internal sealed class VariableContext
    {
        private readonly Dictionary<int, Type> types = new Dictionary<int, Type>();
        private int index;

        public VariableType CreateVariable() =>
            Type.Variable(this.index++);

        public Type? GetInferType(VariableType variableType) =>
            this.types.TryGetValue(variableType.Index, out var type) ? type : null;

        public void AddInferType(VariableType variableType, Type type) =>
            this.types.Add(variableType.Index, type);

        public Type Resolve(Type type)
        {
            if (type is FunctionType functionType)
            {
                return Type.Function(
                    this.Resolve(functionType.ParameterType),
                    this.Resolve(functionType.ExpressionType));
            }
            if (type is VariableType variableType)
            {
                if (types.TryGetValue(variableType.Index, out var vt))
                {
                    return this.Resolve(vt);
                }
            }
            return type;
        }
    }
}
