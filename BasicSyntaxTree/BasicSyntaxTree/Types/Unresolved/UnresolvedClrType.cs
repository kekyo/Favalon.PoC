namespace BasicSyntaxTree.Types.Unresolved
{
    public class UnresolvedClrType : UnresolvedType
    {
        internal UnresolvedClrType(System.Type type) =>
            this.Type = type;

        public System.Type Type { get; }

        public override bool IsResolved => true;

        public override bool Equals(Type other) =>
            other is UnresolvedClrType rhs1 ? this.Type.Equals(rhs1.Type) :
            other is ClrType rhs2 ? this.Type.Equals(rhs2.Type) :
            false;

        public TypedType ToClrType() =>
            new ClrType(this.Type);

        public override string ToString() =>
            this.Type.Name;
    }
}
