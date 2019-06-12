namespace BasicSyntaxTree.Types
{
    public sealed class RuntimeType : Type
    {
        internal RuntimeType(System.Type type) =>
            this.Type = type;

        public System.Type Type { get; }

        public override bool IsResolved => true;

        public override bool Equals(Type other) =>
            other is RuntimeType rhs ? this.Type.Equals(rhs.Type) :
            false;

        public override string ToString() =>
            this.Type.PrettyPrint();
    }
}
