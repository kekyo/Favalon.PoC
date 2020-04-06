using System.Reflection;
using Favalon.Terms.Types;

namespace Favalon
{
    public static class ClrEnvironmentFactory
    {
        public static Environment Create(int iterations = EnvironmentFactory.DefaultIterations) =>
            Environment.Pure(ClrTypeCalculator.Instance, iterations).
            BindMutableClrPublicTypes(typeof(object).GetAssembly()).
            BindMutableClrAliasTypes().
            BindMutableClrTypeOperators().
            BindMutableClrConstants();
    }
}
