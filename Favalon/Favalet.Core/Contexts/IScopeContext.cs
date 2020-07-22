using Favalet.Expressions;

namespace Favalet.Contexts
{
    public interface IScopeContext
    {
        IExpression? LookupVariable(string symbol);
    }
}
