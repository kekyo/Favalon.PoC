using Favalon.Internal;
using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Diagnostics;

namespace Favalon.ParseRunners
{
    internal sealed class WaitingRunner : ParseRunner
    {
        private WaitingRunner()
        { }

        public override ParseRunnerResult Run(ParseRunnerContext context, Token token)
        {
            Debug.Assert(context.CurrentTerm == null);

            switch (token)
            {
                case IdentityToken identity:
                    context.CurrentTerm = new IdentityTerm(identity.Identity);
                    return ParseRunnerResult.Empty(
                        ApplyingRunner.Instance);

                default:
                    throw new InvalidOperationException();
            }
        }

        public static readonly ParseRunner Instance = new WaitingRunner();
    }
}
