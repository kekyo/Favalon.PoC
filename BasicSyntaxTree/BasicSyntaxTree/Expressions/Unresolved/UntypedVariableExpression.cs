using BasicSyntaxTree.Untyped;
using BasicSyntaxTree.Untyped.Types;

namespace BasicSyntaxTree.Expressions.Unresolved
{
    public sealed class UntypedVariableExpression : UntypedExpression
    {
        public readonly string Name;

        internal UntypedVariableExpression(string name, UntypedType? annotatedType, TextRegion textRegion) : base(annotatedType, textRegion) =>
            this.Name = name;

        internal override bool IsSafePrintable =>
            this.AnnotetedType == null;

        internal override TypedExpression Visit(Environment environment, InferContext context)
        {
            if (this.AnnotetedType is Type type)
            {
                environment.RegisterVariable(this.Name, type);
            }
            else if (environment.GetType(this.Name) is Type it)
            {
                //if (it is UntypedTypeConstructor tc)
                //{
                //    var pt = context.CreateUnspecifiedType();
                //    type = new ApplyExpression()
                //}
                //else
                {
                    type = it;
                }
            }
            else
            {
                type = context.CreateUnspecifiedType();
                environment.RegisterVariable(this.Name, type);
            }

            return new VariableExpression(this.Name, type, this.TextRegion);
        }

        public override string ToString() =>
            this.AnnotetedType is Type annotatedType ? $"{this.Name}:{annotatedType}" : this.Name;

        // =======================================================================
        // Short generator usable for tests.

        public static implicit operator UntypedVariableExpression(string variableName) =>
            new UntypedVariableExpression(variableName, default, TextRegion.Unknown);
    }
}
