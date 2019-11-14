using System;

namespace Favalon.Expressions
{
    public interface ITypeExpression
    {
        Type Type { get; }
    }

    public sealed class TypeExpression<TType> :
        VariableExpression<TypeExpression<TType>>, ITypeExpression
    {
        private static readonly Type type = typeof(TType);

        private TypeExpression()
        { }

        public override string SymbolName =>
            type.FullName;

        public override Expression HigherOrder =>
            KindExpression.Instance;

        public Type Type =>
            type;

        protected override Expression VisitResolve(IInferContext context) =>
            this;

        public override bool Equals(Expression? rhs) =>
            rhs is TypeExpression<TType>;

        public static readonly Expression Instance = new TypeExpression<TType>();
    }
}
