using BasicSyntaxTree.Types;
using System.Collections.Generic;
using System.Linq;

namespace BasicSyntaxTree.Expressions
{
    public abstract class Expression
    {
        protected Expression() { }

        public static IReadOnlyDictionary<string, Type> CreateEnvironment(
            params (string name, Type type)[] environments) =>
            environments.ToDictionary(entry => entry.name, entry => entry.type);
    }
}
