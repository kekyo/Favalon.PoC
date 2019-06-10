using BasicSyntaxTree.Typed;
using BasicSyntaxTree.Typed.Types;

namespace BasicSyntaxTree.Untyped.Types
{
    public sealed class UntypedClrType : UntypedType
    {
        internal UntypedClrType(System.Type type) =>
            this.Type = type;

        public System.Type Type { get; }

        public override bool IsResolved => true;

        public override bool Equals(Type other) =>
            other is UntypedClrType rhs1 ? this.Type.Equals(rhs1.Type) :
            other is ClrType rhs2 ? this.Type.Equals(rhs2.Type) :
            false;

        public TypedType ToClrType() =>
            new ClrType(this.Type);

        public override string ToString() =>
            this.Type.Name;
    }
}
