using System.Diagnostics;

namespace BasicSyntaxTree.Types
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public abstract class Type : System.IEquatable<Type>
    {
        private protected Type() { }

        public abstract bool IsResolved { get; }

        public abstract bool Equals(Type other);

        public string DebuggerDisplay =>
            $"{this.GetType().Name}: {this.ToString()}";

        // =======================================================================

        public static Type Runtime<T>() =>
            Runtime(typeof(T));

        public static Type Runtime(System.Type type) =>
            type.IsGenericTypeDefinition ? (Type)new TypeConstructorType(type) : new RuntimeType(type);

        public static KindType Kind<T>() =>
            Kind(typeof(T));

        public static KindType Kind(System.Type type) =>
            type.IsGenericTypeDefinition ? (KindType)new TypeConstructorType(type) : new RuntimeKindType(type);

        public static Type Function(Type parameterType, Type resultType) =>
            ((parameterType is KindType pkt) && (resultType is KindType rkt)) ?
                (Type)KindFunction(pkt, rkt) :
                new FunctionType(parameterType, resultType);

        public static KindType KindFunction(KindType parameterType, KindType resultType) =>
            new KindFunctionType(parameterType, resultType);
    }
}
