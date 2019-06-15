namespace BasicSyntaxTree.Types
{
    public abstract class KindType : Type
    {
        private protected KindType() { }

        public override bool IsResolved => false;
    }
}
