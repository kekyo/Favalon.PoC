using Favalet.Expressions;

namespace Favalet
{
    public interface IReduceContext :
        IScopeContext
    {
        IReduceContext NewScope(string symbol, IExpression expression);
    }
}
