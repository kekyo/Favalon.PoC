﻿using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions
{
    public sealed class ConstantExpression : ResolvedExpression
    {
        public readonly object Value;

        internal ConstantExpression(object value, Type type, TextRegion textRegion) : base(type, textRegion) =>
            this.Value = value;

        internal override bool IsSafePrintable => this.Value is string;

        internal override void Resolve(InferContext context) =>
            this.Type = context.ResolveType(this.Type);

        public override string ToString() =>
            this.Value is string str ? $"\"{str}\"" : $"{this.Value}:{this.Type}";
    }
}
