using BasicSyntaxTree.Types;

namespace BasicSyntaxTree.Expressions.Unresolved
{
    public sealed class UnresolvedVariableExpression : UnresolvedExpression
    {
        public readonly string Name;

        internal UnresolvedVariableExpression(string name, Type? annotatedType, TextRegion textRegion) :
            base(annotatedType, textRegion) =>
            this.Name = name;

        internal override bool IsSafePrintable =>
            this.AnnotetedType == null;

        internal override ResolvedExpression Visit(Environment environment, InferContext context)
        {
            if (this.AnnotetedType is Type type)
            {
                environment.RegisterVariable(this.Name, type);

                if (type is KindType kindType)
                {
                    return new KindTypeExpression(kindType, this.TextRegion);
                }
                else
                {
                    return new VariableExpression(this.Name, type, this.TextRegion);
                }
            }
            else if (environment.GetType(this.Name) is Type inferredType)
            {
                if (inferredType is KindType kindType)
                {
                    return new KindTypeExpression(kindType, this.TextRegion);
                }
                else
                {
                    return new VariableExpression(this.Name, inferredType, this.TextRegion);
                }
            }
            else
            {
                type = context.CreateUnspecifiedType();
                environment.RegisterVariable(this.Name, type);

                return new VariableExpression(this.Name, type, this.TextRegion);
            }
        }

        public override string ToString() =>
            this.AnnotetedType is Type annotatedType ?
                ((this.Name.Length >= 1) ? $"{this.Name}:{annotatedType}" : annotatedType.ToString()) :
                (this.Name.Length >= 1) ? this.Name : "(unnamed)";

        // =======================================================================
        // Short generator usable for tests.

        public static implicit operator UnresolvedVariableExpression(string variableName) =>
            new UnresolvedVariableExpression(variableName, default, TextRegion.Unknown);
    }
}
