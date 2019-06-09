using BasicSyntaxTree.Typed;
using BasicSyntaxTree.Typed.Types;
using BasicSyntaxTree.Untyped.Types;
using System.Collections.Generic;

namespace BasicSyntaxTree.Untyped
{
    public abstract class UntypedExpression : Expression
    {
        private protected UntypedExpression(TextRegion textRegion) : base(textRegion) { }

        public override bool IsResolved => false;

        internal abstract TypedExpression Visit(TypeEnvironment environment, InferContext context);

        public TypedExpression Infer<T>(T typeEnvironment) where T : IReadOnlyDictionary<string, UntypedType>
        {
            var context = new InferContext();
            var typedExpression = this.Visit(new TypeEnvironment(typeEnvironment), context);
            typedExpression.Resolve(context);
            return typedExpression;
        }
    }
}
