namespace Favalon.Expressions
{
    public sealed class TypeExpression : IdentityExpression
    {
        internal TypeExpression(string name, TextRange textRange) :
            base(KindExpression.Instance, textRange) =>
            this.Name = name;

        internal TypeExpression(System.Type type, TextRange textRange) :
            base(KindExpression.Instance, textRange) =>
            this.Name = type.FullName;

        public override string Name { get; }
    }
}
