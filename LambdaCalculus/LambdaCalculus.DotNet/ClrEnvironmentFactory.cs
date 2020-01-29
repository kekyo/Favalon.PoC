using System.Reflection;

namespace Favalon
{
    public static class ClrEnvironmentFactory
    {
        public static Environment Create(int iterations = EnvironmentFactory.DefaultIterations) =>
            Environment.Pure(iterations).
            BindMutableClrPublicTypes(typeof(object).GetAssembly()).
            BindMutableClrAliasTypes().
            BindMutableClrTypeOperators().
            BindMutableClrConstants();
    }
}
