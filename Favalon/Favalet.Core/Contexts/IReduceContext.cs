using Favalet.Expressions;

namespace Favalet.Contexts
{
    public interface IReduceContext :
        IScopeContext
    {
        IReduceContext NewScope(string symbol, IExpression expression);
    }
}
