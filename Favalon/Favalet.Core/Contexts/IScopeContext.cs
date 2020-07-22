using Favalet.Expressions;
using Favalet.Expressions.Algebraic;

namespace Favalet.Contexts
{
    public interface IScopeContext
    {
        ILogicalCalculator TypeCalculator { get; }

        IExpression? LookupVariable(IIdentityTerm identity);
    }
}
