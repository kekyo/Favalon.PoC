using BasicSyntaxTree.Types;
using System.Collections.Generic;

namespace BasicSyntaxTree.Expressions
{
    internal sealed class VariableContext
    {
        private readonly Dictionary<int, Type> types = new Dictionary<int, Type>();
        private int index;

        public UntypedType CreateUntypedType() =>
            Type.Variable(this.index++);

        public Type? GetInferredType(UntypedType variableType) =>
            this.types.TryGetValue(variableType.Index, out var type) ? type : null;

        public void AddInferredType(UntypedType variableType, Type type) =>
            this.types.Add(variableType.Index, type);

        public Type ResolveType(Type type)
        {
            if (type is FunctionType functionType)
            {
                return Type.Function(
                    this.ResolveType(functionType.ParameterType),
                    this.ResolveType(functionType.ExpressionType));
            }
            if (type is UntypedType variableType)
            {
                if (types.TryGetValue(variableType.Index, out var vt))
                {
                    return this.ResolveType(vt);
                }
            }
            return type;
        }
    }
}
