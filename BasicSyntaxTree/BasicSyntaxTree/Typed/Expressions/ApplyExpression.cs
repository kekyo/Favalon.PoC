using BasicSyntaxTree.Untyped;
using BasicSyntaxTree.Untyped.Types;

namespace BasicSyntaxTree.Typed.Expressions
{
    public sealed class ApplyExpression : TypedExpression
    {
        public new readonly TypedExpression Lambda;
        public readonly TypedExpression Argument;

        internal ApplyExpression(TypedExpression lambda, TypedExpression argument, UntypedType type, TextRegion textRegion) : base(type, textRegion)
        {
            this.Lambda = lambda;
            this.Argument = argument;
        }

        internal override void Resolve(InferContext context)
        {
            this.Lambda.Resolve(context);
            this.Argument.Resolve(context);
            this.Type = context.ResolveType(this.Type);
        }

        public override string ToString() =>
            $"({this.Lambda} {this.Argument}):{this.Type}";
    }
}
