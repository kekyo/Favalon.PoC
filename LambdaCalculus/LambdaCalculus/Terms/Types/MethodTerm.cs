using Favalon.Contexts;
using Favalon.Terms.Algebric;
using LambdaCalculus.Contexts;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalon.Terms.Types
{
    public abstract class MethodTerm : HigherOrderLazyTerm
    {
        private protected MethodTerm()
        { }

        private protected static LambdaTerm GetMethodHigherOrder(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return LambdaTerm.From(
                TypeTerm.From((parameters.Length == 0) ? typeof(void) : parameters[0].ParameterType),
                TypeTerm.From(method.ReturnType));
        }

        public static MethodTerm From(IEnumerable<MethodInfo> methods)
        {
            var ms = methods.ToArray();
            return (ms.Length == 1) ?
                (MethodTerm)new ClrMethodTerm(ms[0]) :
                new ClrMethodOverloadedTerm(ms.Select(method => new ClrMethodTerm(method)).ToArray());
        }
    }
}
