using BasicSyntaxTree.Untyped.Expressions;
using System.Collections.Generic;

namespace BasicSyntaxTree
{
    public sealed class Environment
    {
        private Dictionary<string, Type> types;
        private bool isClonedTypes;

        public Environment()
        {
            this.types = new Dictionary<string, Type>();
            this.isClonedTypes = true;
        }

        private Environment(Dictionary<string, Type> types)
        {
            this.types = types;
        }

        public Environment MakeScope() =>
            new Environment(this.types);

        public void RegisterVariable(string name, Type type)
        {
            if (!this.isClonedTypes)
            {
                this.types = new Dictionary<string, Type>(this.types);
                this.isClonedTypes = true;
            }

            this.types[name] = type;
        }

        public Type? GetType(string name) =>
            this.types.TryGetValue(name, out var type) ? type : null;
    }
}
