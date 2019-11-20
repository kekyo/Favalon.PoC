using Favalon.Terms;
using Favalon.Tokens;
using System.Globalization;
using System.Linq;

namespace Favalon.ParseRunners
{
    internal abstract class ParseRunner
    {
        protected ParseRunner()
        { }

        public abstract ParseRunnerResult Run(ParseRunnerContext context, Token token);
    }
}
