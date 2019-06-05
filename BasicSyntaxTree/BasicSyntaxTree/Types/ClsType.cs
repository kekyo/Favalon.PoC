namespace BasicSyntaxTree.Types
{
    public abstract class ClsTypeBase : Type
    {
        private protected ClsTypeBase() { }

        public abstract System.Type Type { get; }

        public override bool Equals(Type other) =>
            other is ClsTypeBase rhs ? this.Type.Equals(rhs.Type) : false;

        public override string ToString() =>
            this.Type.Name;
    }

    public sealed class ClsType : ClsTypeBase
    {
        internal ClsType(System.Type type) =>
            this.Type = type;

        public override System.Type Type { get; }
    }

    public sealed class ClsType<T> : ClsTypeBase
    {
        internal ClsType() { }

        public override System.Type Type => typeof(T);
    }
}
