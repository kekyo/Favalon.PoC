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

            switch (token)
            {
                case NumericToken numeric:
                    context.CurrentTerm = CombineTerms(
                        context.CurrentTerm,
                        GetNumericConstant(numeric.Value, context.PreSignToken!.Sign));
                    context.PreSignToken = null;
                    return ParseRunnerResult.Empty(
                        ApplyingRunner.Instance);

                case WhiteSpaceToken _:
                    context.CurrentTerm = CombineTerms(
                        context.CurrentTerm,
                        new IdentityTerm(context.PreSignToken!.Symbol.ToString()));
                    context.PreSignToken = null;
                    return ParseRunnerResult.Empty(
                        ApplyingRunner.Instance);

                case IdentityToken identity:
                    context.CurrentTerm = CombineTerms(
                        context.CurrentTerm,
                        new IdentityTerm(context.PreSignToken!.Symbol.ToString()),
                        new IdentityTerm(identity.Identity));
                    return ParseRunnerResult.Empty(this);

                default:
                    throw new InvalidOperationException();
            }
        }

        public static readonly ParseRunner Instance = new NumericalSignedRunner();
    }
}
