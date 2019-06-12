namespace BasicSyntaxTree.Types
{
    public abstract class KindType : Type
    {
        private protected KindType(System.Type type) =>
            this.Type = type;

        public System.Type Type { get; }

        public override bool IsResolved => false;

        public override string ToString() =>
            this.Type.PrettyPrint();
    }
}
