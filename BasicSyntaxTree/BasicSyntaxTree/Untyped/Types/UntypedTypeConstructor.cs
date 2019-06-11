namespace BasicSyntaxTree.Untyped.Types
{
    public sealed class UntypedTypeConstructor : UntypedClrType
    {
        internal UntypedTypeConstructor(System.Type typeConstructor) : base(typeConstructor)
        {
        }

        public override bool IsResolved => false;

        public override string ToString() =>
            $"{this.Type.Name}<>";
    }
}
