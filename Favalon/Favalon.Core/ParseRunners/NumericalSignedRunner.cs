using Favalon.Internal;
using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Diagnostics;

namespace Favalon.ParseRunners
{
    internal sealed class NumericalSignedRunner : ParseRunner
    {
        private NumericalSignedRunner()
        { }

        public override ParseRunnerResult Run(ParseRunnerContext context, Token token)
        {
            Debug.Assert(context.PreSignToken != null);
            Debug.Assert(context.ApplyRightToLeft == false);

            switch (token)
            {
                // "-123"
                case NumericToken numeric:
                    context.CurrentTerm = Utilities.CombineTerms(
                        context.CurrentTerm,
                        Utilities.GetNumericConstant(numeric.Value, context.PreSignToken!.Sign));
                    context.PreSignToken = null;
                    return ParseRunnerResult.Empty(
                        ApplyingRunner.Instance);

                // "- ..."
                case WhiteSpaceToken _:
                    // Will make binary op
                    context.CurrentTerm = Utilities.CombineTerms(
                        context.CurrentTerm,
                        new IdentityTerm(context.PreSignToken!.Symbol.ToString()));
                    context.PreSignToken = null;
                    return ParseRunnerResult.Empty(
                        ApplyingRunner.Instance);

                // "-abc"
                case IdentityToken identity:
                    // Will make binary op
                    context.CurrentTerm = Utilities.CombineTerms(
                        context.CurrentTerm,
                        new IdentityTerm(context.PreSignToken!.Symbol.ToString()),
                        new IdentityTerm(identity.Identity));
                    context.PreSignToken = null;
                    return ParseRunnerResult.Empty(
                        ApplyingRunner.Instance);

                default:
                    throw new InvalidOperationException(token.ToString());
            }
        }

        public static readonly ParseRunner Instance = new NumericalSignedRunner();
    }
}
