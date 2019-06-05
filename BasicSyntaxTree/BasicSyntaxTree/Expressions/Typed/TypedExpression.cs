using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Typed
{
    public abstract class TypedExpression : Expression
    {
        protected TypedExpression(Type type) =>
            this.Type = type;

        public Type Type { get; private protected set; }

        internal virtual void Resolve(VariableContext context) =>
            this.Type = context.ResolveType(this.Type);
    }
}
