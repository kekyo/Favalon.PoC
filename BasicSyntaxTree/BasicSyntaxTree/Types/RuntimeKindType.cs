namespace BasicSyntaxTree.Types
{
    public sealed class RuntimeKindType : KindType
    {
        internal RuntimeKindType(System.Type type) :
            base(type) { }

        public override bool Equals(Type other) =>
            other is RuntimeKindType rhs ? this.Type.Equals(rhs.Type) :
            false;
    }
}
