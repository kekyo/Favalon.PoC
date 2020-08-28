using System.Diagnostics;
using Favalet.Expressions;

namespace Favalet.Contexts.Unifiers
{
    [DebuggerStepThrough]
    internal readonly struct Unification
    {
        public readonly IExpression Expression;
        public readonly bool Fixed;

        private Unification(IExpression expression, bool @fixed)
        {
            this.Expression = expression;
            this.Fixed = @fixed;
        }

        public string ToString(PrettyStringTypes type)
        {
            var @fixed = this.Fixed ? "Fixed," : string.Empty;
            return $"{@fixed}{this.Expression.GetPrettyString(type)}";
        }
        public override string ToString() =>
            this.ToString(PrettyStringTypes.Readable);

        public static Unification Create(IExpression expression, bool @fixed) =>
            new Unification(expression, @fixed);
    }
}
