namespace Favalon.Expressions
{
    public sealed class TypeExpression : IdentityExpression
    {
        internal TypeExpression(string name) :
            base(KindExpression.Instance) =>
            this.Name = name;

        public override string Name { get; }

        internal override bool IsIgnoreAnnotationReadableString =>
            true;
    }
}
