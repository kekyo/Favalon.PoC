using System;
using System.Runtime.CompilerServices;

namespace Favalon.Expressions
{
    public abstract class Expression :
        Inferable, IEquatable<Expression?>
    {
        protected Expression()
        { }

        public abstract Expression HigherOrder { get; }

        protected abstract Expression VisitResolve(IInferContext context);

        public abstract bool Equals(Expression? rhs);

        public override bool Equals(object? rhs) =>
            this.Equals(rhs as Expression);

#line hidden
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Expression VisitResolveCore(IInferContext context) =>
            this.VisitResolve(context);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Expression VisitResolve(IInferContext context, Expression targetExpression) =>
            targetExpression.VisitResolveCore(context);
#line default
    }
}
