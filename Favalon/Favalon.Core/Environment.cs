using Favalon.Terms;

namespace Favalon
{
    public sealed class Environment : Context
    {
        private Environment()
        { }

        public static Environment Create() =>
            new Environment();
    }
}
