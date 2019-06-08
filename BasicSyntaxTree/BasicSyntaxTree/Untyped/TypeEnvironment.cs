using BasicSyntaxTree.Untyped.Types;
using System.Collections.Generic;

namespace BasicSyntaxTree.Untyped
{
    internal sealed class TypeEnvironment
    {
        private Dictionary<string, UntypedType> types;
        private bool isCloned;

        public TypeEnvironment(IReadOnlyDictionary<string, UntypedType> types)
        {
            this.types = types.ToDictionary();
            this.isCloned = true;
        }

        private TypeEnvironment(Dictionary<string, UntypedType> types) =>
            this.types = types;

        public TypeEnvironment MakeScope() =>
            new TypeEnvironment(this.types);

        public void RegisterVariable(string name, UnspecifiedType variableType)
        {
            if (!this.isCloned)
            {
                this.types = new Dictionary<string, UntypedType>(this.types);
                this.isCloned = true;
            }

            this.types[name] = variableType;
        }

        public UntypedType? GetType(string name) =>
            this.types.TryGetValue(name, out var type) ? type : null;
    }
}
