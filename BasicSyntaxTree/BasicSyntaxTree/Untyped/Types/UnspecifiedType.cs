namespace BasicSyntaxTree.Untyped.Types
{
    public sealed class UnspecifiedType : UntypedType
    {
        public readonly int Index;

        internal UnspecifiedType(int index) =>
            this.Index = index;

        public override bool IsResolved => false;

        public override bool Equals(Type other) =>
            other is UnspecifiedType rhs ?
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
