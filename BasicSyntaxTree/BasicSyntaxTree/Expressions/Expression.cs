using BasicSyntaxTree.Types;
using System.Collections.Generic;
using System.Linq;

namespace BasicSyntaxTree.Expressions
{
    public abstract class Expression
    {
        public readonly TextRegion TextRegion;

        protected Expression(TextRegion textRegion) =>
            this.TextRegion = textRegion;

        public static IReadOnlyDictionary<string, Type> CreateEnvironment(
            params (string name, Type type)[] environments) =>
            environments.ToDictionary(entry => entry.name, entry => entry.type);
    }
}
