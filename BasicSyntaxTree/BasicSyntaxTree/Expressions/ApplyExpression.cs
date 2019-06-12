using BasicSyntaxTree.Untyped;
using BasicSyntaxTree.Untyped.Types;

namespace BasicSyntaxTree.Expressions
{
    public sealed class ApplyExpression : TypedExpression
    {
        public readonly TypedExpression Function;
        public readonly TypedExpression Argument;

        internal ApplyExpression(TypedExpression function, TypedExpression argument, Type type, TextRegion textRegion) : base(type, textRegion)
        {
            this.Function = function;
            this.Argument = argument;
        }

        internal override bool IsSafePrintable => false;

        internal override void Resolve(InferContext context)
        {
            this.Function.Resolve(context);
            this.Argument.Resolve(context);
            this.Type = context.ResolveType(this.Type);
        }

        public override string ToString() =>
            $"({this.Function} {this.Argument.SafePrintable}):{this.Type}";
    }
}
