namespace BasicSyntaxTree.Types
{
    public sealed class TypeConstructorType : KindType, IRuntimeType
    {
        internal TypeConstructorType(System.Type type) =>
            this.Type = type;

        public System.Type Type { get; }

        public override bool Equals(Type other) =>
            other is TypeConstructorType rhs ? this.Type.Equals(rhs.Type) :
            false;

        public override string ToString() =>
            this.Type.PrettyPrint();

        internal Type Apply(KindType argumentType) =>
            Kind((argumentType is IRuntimeType art) ?
                this.Type.MakeGenericType(art.Type) :
                throw new System.InvalidOperationException());
    }
}
