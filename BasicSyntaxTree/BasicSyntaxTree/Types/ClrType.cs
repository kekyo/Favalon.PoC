using BasicSyntaxTree.Types.Unresolved;

namespace BasicSyntaxTree.Types
{
    public sealed class ClrType : TypedType
    {
        internal ClrType(System.Type type) =>
            this.Type = type;

        public System.Type Type { get; }

        public override bool IsResolved => true;

        public override bool Equals(Type other) =>
            other is UnresolvedClrType rhs1 ? this.Type.Equals(rhs1.Type) :
            other is ClrType rhs2 ? this.Type.Equals(rhs2.Type) :
            false;

        public override string ToString() =>
            this.Type.Name;
    }
}
