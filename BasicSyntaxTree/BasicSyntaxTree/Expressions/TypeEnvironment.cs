using BasicSyntaxTree.Types;
using System.Collections.Generic;

namespace BasicSyntaxTree.Expressions
{
    internal sealed class TypeEnvironment
    {
        private Dictionary<string, Type> types;
        private bool isCloned;

        public TypeEnvironment(IReadOnlyDictionary<string, Type> types)
        {
            this.types = types.ToDictionary();
            this.isCloned = true;
        }

        private TypeEnvironment(Dictionary<string, Type> types) =>
            this.types = types;

        public TypeEnvironment MakeScope() =>
            new TypeEnvironment(this.types);

        public void RegisterVariable(string name, UntypedType variableType)
        {
            if (!this.isCloned)
            {
                this.types = new Dictionary<string, Type>(this.types);
                this.isCloned = true;
            }

            this.types[name] = variableType;
        }

        public Type? GetType(string name) =>
            this.types.TryGetValue(name, out var type) ? type : null;
    }
}
