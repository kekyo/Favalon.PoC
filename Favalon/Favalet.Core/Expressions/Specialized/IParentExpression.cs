using System.Collections.Generic;

namespace Favalet.Expressions.Specialized
{
    public interface IParentExpression :
        IExpression
    {
        IEnumerable<IExpression> Children { get; }
    }
}
