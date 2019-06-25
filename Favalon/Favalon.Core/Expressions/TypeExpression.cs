namespace Favalon.Expressions
{
    public sealed class TypeExpression : IdentityExpression
    {
        internal TypeExpression(string name, TextRange textRange) :
            base(KindExpression.Instance, textRange) =>
            this.Name = name;

        public override string Name { get; }
    }
}
