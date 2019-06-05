namespace BasicSyntaxTree.Types
{
    public sealed class IntegerType : Type
    {
        internal IntegerType() { }

        public override bool Equals(Type other) =>
            other is IntegerType;

        public override string ToString() =>
            "Integer";
    }
}
