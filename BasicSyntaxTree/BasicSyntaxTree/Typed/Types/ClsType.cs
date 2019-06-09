using BasicSyntaxTree.Untyped.Types;

namespace BasicSyntaxTree.Typed.Types
{
    public abstract class ClrTypeBase : TypedType
    {
        private protected ClrTypeBase() { }

        public abstract System.Type Type { get; }

        public override bool IsResolved => true;

        public override bool Equals(Type other) =>
            other is UntypedClrTypeBase rhs1 ? this.Type.Equals(rhs1.Type) :
            other is ClrTypeBase rhs2 ? this.Type.Equals(rhs2.Type) :
            false;

        public override string ToString() =>
            this.Type.Name;
    }

    public sealed class ClrType : ClrTypeBase
    {
        internal ClrType(System.Type type) =>
            this.Type = type;

        public override System.Type Type { get; }
    }

    public sealed class ClrType<T> : ClrTypeBase
    {
        private static readonly System.Type type = typeof(T);

        internal ClrType() { }

        public override System.Type Type => type;
    }
}
