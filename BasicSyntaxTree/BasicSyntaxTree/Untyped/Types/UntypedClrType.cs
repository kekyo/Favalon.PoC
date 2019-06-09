using BasicSyntaxTree.Typed;
using BasicSyntaxTree.Typed.Types;

namespace BasicSyntaxTree.Untyped.Types
{
    public abstract class UntypedClrTypeBase : UntypedType
    {
        private protected UntypedClrTypeBase() { }

        public abstract System.Type Type { get; }

        public override bool IsResolved => true;

        public override bool Equals(Type other) =>
            other is UntypedClrTypeBase rhs1 ? this.Type.Equals(rhs1.Type) :
            other is ClrTypeBase rhs2 ? this.Type.Equals(rhs2.Type) :
            false;

        public abstract TypedType ToClrType();

        public override string ToString() =>
            this.Type.Name;
    }

    public sealed class UntypedClrType : UntypedClrTypeBase
    {
        internal UntypedClrType(System.Type type) =>
            this.Type = type;

        public override System.Type Type { get; }

        public override TypedType ToClrType() =>
            new ClrType(this.Type);
    }

    public sealed class UntypedClrType<T> : UntypedClrTypeBase
    {
        private static readonly System.Type type = typeof(T);

        private UntypedClrType() { }

        public override System.Type Type => type;

        public override TypedType ToClrType() =>
            Typed.Types.ClrType<T>.Instance;

        internal static readonly UntypedClrType<T> Instance =
            new UntypedClrType<T>();
    }
}
