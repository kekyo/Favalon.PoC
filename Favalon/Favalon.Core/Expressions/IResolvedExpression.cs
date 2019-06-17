using System;

namespace Favalon.Expressions
{
    public interface IResolvedExpression
    {
        Expression HigherOrderExpression { get; }
    }
}
