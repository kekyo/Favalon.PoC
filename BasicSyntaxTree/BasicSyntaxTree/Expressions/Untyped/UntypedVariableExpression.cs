﻿using BasicSyntaxTree.Expressions.Typed;
using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Untyped
{
    public sealed class UntypedVariableExpression : UntypedExpression
    {
        public readonly string Name;

        internal UntypedVariableExpression(string name, TextRegion textRegion) : base(textRegion) =>
            this.Name = name;

        internal override TypedExpression Visit(TypeEnvironment environment, InferContext context) =>
            new VariableExpression(this.Name, environment.GetType(this.Name) ?? context.CreateUntypedType(), this.TextRegion);

        public override string ToString() =>
            this.Name;
    }
}
