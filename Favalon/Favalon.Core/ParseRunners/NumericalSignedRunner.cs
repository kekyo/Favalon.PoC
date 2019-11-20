﻿using Favalon.Internal;
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
                    context.CurrentTerm = Utilities.CombineTerms(
                        context.CurrentTerm,
                        Utilities.GetNumericConstant(numeric.Value, context.PreSignToken!.Sign));
                    context.PreSignToken = null;
                    return ParseRunnerResult.Empty(
                        ApplyingRunner.Instance);

                case WhiteSpaceToken _:
                    context.CurrentTerm = Utilities.CombineTerms(
                        context.CurrentTerm,
                        new IdentityTerm(context.PreSignToken!.Symbol.ToString()));
                    context.PreSignToken = null;
                    return ParseRunnerResult.Empty(
                        ApplyingRunner.Instance);

                case IdentityToken identity:
                    context.CurrentTerm = Utilities.CombineTerms(
                        context.CurrentTerm,
                        new IdentityTerm(context.PreSignToken!.Symbol.ToString()),
                        new IdentityTerm(identity.Identity));
                    context.PreSignToken = null;
                    return ParseRunnerResult.Empty(
                        ApplyingRunner.Instance);

                default:
                    throw new InvalidOperationException();
            }
        }

        public static readonly ParseRunner Instance = new NumericalSignedRunner();
    }
}
