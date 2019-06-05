﻿using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Typed
{
    public abstract class TypedExpression : Expression
    {
        private protected TypedExpression(Type type) =>
            this.Type = type;

        public Type Type { get; private set; }

        internal virtual void Resolve(InferContext context) =>
            this.Type = context.ResolveType(this.Type);
    }
}
