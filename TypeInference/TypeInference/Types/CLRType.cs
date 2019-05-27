namespace TypeInference.Types
{
    public sealed class CLRType : AvalonType
    {
        private readonly System.Type type;

        public CLRType(System.Type type) => this.type = type;

        public System.Type RawType => this.type;

        public override int GetHashCode() =>
            this.type.GetHashCode();

        public override bool Equals(AvalonType other) =>
            this.type == (other as CLRType)?.type;
    }
}
