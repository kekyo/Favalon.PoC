using Favalet.Expressions;
using Favalet.Tokens;
using System;
using System.Diagnostics;

namespace Favalet.Parsers
{
    internal sealed class NumericalSignedRunner : ParseRunner
    {
        private NumericalSignedRunner()
        { }

        public override ParseRunnerResult Run(ParseRunnerContext context, Token token)
        {
            //Debug.Assert(context.PreSignToken != null);
            //Debug.Assert(context.ApplyNextAssociative == BoundTermAssociatives.LeftToRight);

            switch (token)
            {
                // // "-123"
                // case NumericToken numeric:
                //     context.CombineAfter(
                //         ParserUtilities.GetNumericConstant(numeric.Value, context.PreSignToken!.Sign));
                //     context.PreSignToken = null;
                //     return ParseRunnerResult.Empty(
                //         ApplyingRunner.Instance);
                //
                // // "- ..."
                // case WhiteSpaceToken _:
                //     // Initial precedence (ArithmericAddition)
                //     context.SetPrecedence(BoundTermPrecedences.ArithmericAddition);
                //
                //     // Will make binary op
                //     context.CombineAfter(
                //         new IdentityTerm(context.PreSignToken!.Symbol.ToString()));
                //     context.PreSignToken = null;
                //     return ParseRunnerResult.Empty(
                //         ApplyingRunner.Instance);
                //
                // // "-abc"
                // case IdentityToken identity:
                //     // Initial precedence (ArithmericAddition)
                //     context.SetPrecedence(BoundTermPrecedences.ArithmericAddition);
                //
                //     // Will make binary op
                //     context.CombineAfter(
                //         new IdentityTerm(context.PreSignToken!.Symbol.ToString()));
                //     context.CombineAfter(
                //         new IdentityTerm(identity.Identity));
                //     context.PreSignToken = null;
                //     return ParseRunnerResult.Empty(
                //         ApplyingRunner.Instance);

                default:
                    throw new InvalidOperationException(token.ToString());
            }
        }

        public static readonly ParseRunner Instance =
            new NumericalSignedRunner();
    }
}
