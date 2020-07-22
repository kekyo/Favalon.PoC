using Favalet.Expressions;

namespace Favalet.Contexts
{
    public interface IReduceContext :
        IScopeContext
    {
        IReduceContext NewScope(IIdentityTerm parameter, IExpression expression);
    }
}
