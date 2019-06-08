using BasicSyntaxTree.Types;
using System.Collections.Generic;
using System.Linq;

namespace BasicSyntaxTree.Untyped
{
    internal sealed class InferContext
    {
        private readonly Dictionary<int, Type> types = new Dictionary<int, Type>();
        private int index;

        public UntypedType CreateUntypedType() =>
            Type.Untyped(this.index++);

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

        public override string ToString() =>
            $"[{string.Join(",", this.types.Select(entry => $"{{{entry.Key}:{entry.Value}}}"))}]";
    }
}
