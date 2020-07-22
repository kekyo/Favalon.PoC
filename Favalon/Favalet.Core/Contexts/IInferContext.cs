using Favalet.Expressions;

namespace Favalet.Contexts
{
    public interface IInferContext :
        IScopeContext
    {
        IInferContext NewScope(IIdentityTerm parameter, IExpression expression);
    }
}
