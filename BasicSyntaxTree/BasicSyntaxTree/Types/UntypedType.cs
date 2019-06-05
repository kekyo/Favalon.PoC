namespace BasicSyntaxTree.Types
{
    public sealed class UntypedType : Type
    {
        public readonly int Index;

        internal UntypedType(int index) =>
            this.Index = index;

        public override bool Equals(Type other) =>
            other is UntypedType rhs ?
                (this.Index == rhs.Index) :
                false;

        public override string ToString()
        {
            var ch = (char)('a' + (this.Index % ('z' - 'a' + 1)));
            var suffixIndex = this.Index / ('z' - 'a' + 1);
            var suffix = (suffixIndex >= 1) ? suffixIndex.ToString() : string.Empty;
            return $"'{ch}{suffix}";
        }
    }
}
