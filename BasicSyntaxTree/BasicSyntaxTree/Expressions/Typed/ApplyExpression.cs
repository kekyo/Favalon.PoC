using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Typed
{
    public sealed class ApplyExpression : TypedExpression
    {
        public readonly TypedExpression Lambda;
        public readonly TypedExpression Argument;

        internal ApplyExpression(TypedExpression lambda, TypedExpression argument, Type type) : base(type)
        {
            this.Lambda = lambda;
            this.Argument = argument;
        }

        internal override void Resolve(InferContext context)
        {
            this.Lambda.Resolve(context);
            this.Argument.Resolve(context);
            base.Resolve(context);
        }

        public override string ToString() =>
            $"({this.Lambda} {this.Argument}):{this.Type}";
    }
}
