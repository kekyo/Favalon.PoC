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

        internal Type Apply(KindType argumentType) =>
            Kind((this.Type is IRuntimeType rt1) && (argumentType is IRuntimeType rt2) ?
                rt1.Type.MakeGenericType(rt2.Type) :
                throw new System.InvalidOperationException());
    }
}
