namespace BasicSyntaxTree.Types
{
    public sealed class DotnetType : Type
    {
        public readonly System.Type Type;

        internal DotnetType(System.Type type) =>
            this.Type = type;

        public override bool Equals(Type other) =>
            other is DotnetType;

        public override string ToString() =>
            this.Type.Name;
    }
}
