using System.Reflection;

namespace Favalon
{
    public static class ClrEnvironmentFactory
    {
        public static Environment Create(int iterations = EnvironmentFactory.DefaultIterations) =>
            Environment.Pure(iterations).
            BindClrPublicTypes(typeof(object).GetAssembly()).
            BindClrAliasTypes().
            BindClrTypeOperators().
            BindClrConstants();
    }
}
