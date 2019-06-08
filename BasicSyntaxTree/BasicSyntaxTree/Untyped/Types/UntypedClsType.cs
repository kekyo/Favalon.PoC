namespace BasicSyntaxTree.Untyped.Types
{
    public abstract class UntypedClsTypeBase : UntypedType
    {
        private protected UntypedClsTypeBase() { }

        public abstract System.Type Type { get; }

        public override bool IsResolved => true;

        public override bool Equals(Type other) =>
            other is UntypedClsTypeBase rhs ? this.Type.Equals(rhs.Type) : false;

        public override string ToString() =>
            this.Type.Name;
    }

    public sealed class UntypedClsType : UntypedClsTypeBase
    {
        internal UntypedClsType(System.Type type) =>
            this.Type = type;

        public override System.Type Type { get; }
    }

    public sealed class UntypedClsType<T> : UntypedClsTypeBase
    {
        private static readonly System.Type type = typeof(T);

        internal UntypedClsType() { }

        public override System.Type Type => type;
    }
}
