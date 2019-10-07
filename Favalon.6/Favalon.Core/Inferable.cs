using Favalon.Expressions;
using System.Collections.Generic;

namespace Favalon
{
    public abstract class Inferable
    {
        internal Inferable()
        { }

        protected internal interface IInferContext
        {
            IReadOnlyList<Expression> Lookup(string symbolName);
            void Unify(Expression expression1, Expression expression2);
        }
    }
}
