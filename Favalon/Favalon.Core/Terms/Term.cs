using Favalon.Expressions;
using System;
using System.Runtime.CompilerServices;

namespace Favalon.Terms
{
    public abstract class Term : IEquatable<Term?>
    {
        protected Term()
        { }

        protected abstract Expression Visit(InferContext context);

#line hidden
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Expression VisitCore(InferContext context) =>
            this.Visit(context);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected Expression Visit(InferContext context, Term targetTerm) =>
            targetTerm.VisitCore(context);
#line default

        public abstract bool Equals(Term? rhs);

        public override bool Equals(object? rhs) =>
            this.Equals(rhs as Term);

        protected internal sealed class InferContext
        {
            public Expression Lookup(string symbolName)
            {
                return null!;
            }
        }
    }
}
