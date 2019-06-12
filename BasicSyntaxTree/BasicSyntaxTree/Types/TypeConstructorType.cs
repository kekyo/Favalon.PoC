namespace BasicSyntaxTree.Types
{
    public sealed class TypeConstructorType : KindType
    {
        internal TypeConstructorType(System.Type type) :
            base(type) { }

        public override bool Equals(Type other) =>
            other is TypeConstructorType rhs ? this.Type.Equals(rhs.Type) :
            false;

        internal Type Apply(KindType argumentType) =>
            KindType(this.Type.MakeGenericType(argumentType.Type));
    }
}
