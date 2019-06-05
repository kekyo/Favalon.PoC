﻿using BasicSyntaxTree.Expressions.Typed;
using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Untyped
{
    public sealed class UntypedLambdaExpression : UntypedExpression
    {
        public readonly string Parameter;
        public readonly UntypedExpression Body;

        internal UntypedLambdaExpression(string parameter, UntypedExpression body, TextRegion textRegion) : base(textRegion)
        {
            this.Parameter = parameter;
            this.Body = body;
        }

        internal override TypedExpression Visit(TypeEnvironment environment, InferContext context)
        {
            var scopedEnvironment = environment.MakeScope();
            var parameterType = context.CreateUntypedType();
            scopedEnvironment.RegisterVariable(this.Parameter, parameterType);
            var body = this.Body.Visit(scopedEnvironment, context);
            var type = Type.Function(parameterType, body.Type);
            return new LambdaExpression(this.Parameter, body, type, this.TextRegion);
        }

        public override string ToString() =>
            $"fun {this.Parameter} = {this.Body}";
    }
}
