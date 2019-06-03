using System.Collections.Generic;
using System.Linq;

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
    }
}
