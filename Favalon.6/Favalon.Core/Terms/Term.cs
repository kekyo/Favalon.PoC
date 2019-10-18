using Favalon.Expressions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Favalon.Terms
{
    public abstract class Term :
        Inferable, IEquatable<Term?>
    {
        protected Term()
        { }

        protected abstract Expression VisitInfer(IInferContext context);

#line hidden
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Expression VisitInferCore(IInferContext context) =>
            this.VisitInfer(context);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Expression Visit(IInferContext context, Term targetTerm) =>
            targetTerm.VisitInferCore(context);
#line default

        public abstract bool Equals(Term? rhs);

        public override bool Equals(object? rhs) =>
            this.Equals(rhs as Term);
    }
}
