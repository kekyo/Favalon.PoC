using Favalet.Expressions.Specialized;

namespace Favalet.Expressions.Additionals
{
    public sealed class TypeExpression : FreeVariableExpression
    {
        internal TypeExpression(string typeName) :
            base(typeName, KindExpression.Instance)
        { }
    }
}
