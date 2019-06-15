using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Unresolved
{
    public sealed class UnresolvedApplyExpression : UnresolvedExpression
    {
        public readonly UnresolvedExpression Function;
        public readonly UnresolvedExpression Argument;

        internal UnresolvedApplyExpression(
            UnresolvedExpression function, UnresolvedExpression argument, Type? annotatedType,
            TextRegion textRegion) : base(annotatedType, textRegion)
        {
            this.Function = function;
            this.Argument = argument;
        }

        internal override bool IsSafePrintable => false;

        internal override ResolvedExpression Visit(Environment environment, InferContext context)
        {
            var function = this.Function.Visit(environment, context);
            var argument = this.Argument.Visit(environment, context);
            var thisExpressionType = this.AnnotetedType;

            // Apply type constructors
            // Apply(tycon, kind) : kind
            if ((function.InferredType is TypeConstructorType tycon) &&
                (argument.InferredType is KindType argumentType))
            {
                thisExpressionType = thisExpressionType ?? tycon.Apply(argumentType);
            }
            // Apply constructor with instance
            // Apply(rkind, 'a) : rkind
            else if ((function.InferredType is RuntimeKindType rkindType) &&
                !(argument.InferredType is KindType))
            {
                thisExpressionType = thisExpressionType ?? rkindType.ToRuntimeType();
            }
            // Likes applying function expression
            else
            {
                thisExpressionType = thisExpressionType ?? context.CreateUnspecifiedType();
            }

            context.Unify(function.InferredType, new FunctionType(argument.InferredType, thisExpressionType));

            return new ApplyExpression(
                function, argument, thisExpressionType, this.TextRegion);
        }

        public override string ToString() =>
            $"{this.Function} {this.Argument.SafePrintable}";
    }
}
