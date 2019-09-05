using System;

namespace Favalon.Expressions
{
    public sealed class KindExpression :
        VariableExpression<KindExpression>
    {
        private KindExpression()
        { }

        public override string SymbolName =>
            "*";

        public override Expression HigherOrder =>
            null!;

        protected override Expression VisitResolve(IInferContext context) =>
            this;

        public override bool Equals(Expression? rhs) =>
            rhs is KindExpression;

        public override string ToString() =>
            $"*";

        public static readonly Expression Instance = new KindExpression();
    }
}
