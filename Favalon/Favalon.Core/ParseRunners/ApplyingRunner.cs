﻿using Favalon.Terms;
using Favalon.Tokens;
using System;
using System.Diagnostics;

namespace Favalon.ParseRunners
{
    internal sealed class ApplyingRunner : ParseRunner
    {
        private ApplyingRunner()
        { }

        public override ParseRunnerResult Run(ParseRunnerContext context, Token token)
        {
            Debug.Assert(context.CurrentTerm != null);
            Debug.Assert(context.PreSignToken == null);

            // Ignore WillApplyRightToLeft checking if token is whitespace.
            if (token is WhiteSpaceToken)
            {
                return ParseRunnerResult.Empty(this);
            }

            // Triggered the token arranging by RTL.
            if (context.WillApplyRightToLeft)
            {
                if (token is ValueToken)
                {
                    context.Scopes.Push(
                        new ScopeInformation(context.CurrentTerm));
                    context.CurrentTerm = null;
                }

                context.WillApplyRightToLeft = false;
            }

            switch (token)
            {
                case IdentityToken identity:
                    if (context.Context.LookupBoundTerms(identity.Identity) is BoundTermInformation[] terms)
                    {
                        if (terms[0].Infix)
                        {
                            if (context.CurrentTerm is ApplyTerm(Term left, Term right))
                            {
                                context.CurrentTerm = ParserUtilities.CombineTerms(
                                    left,
                                    new IdentityTerm(identity.Identity),
                                    right);
                            }
                            else
                            {
                                context.CurrentTerm = ParserUtilities.CombineTerms(
                                    new IdentityTerm(identity.Identity),
                                    context.CurrentTerm);
                            }
                        }
                        else
                        {
                            context.CurrentTerm = ParserUtilities.CombineTerms(
                                context.CurrentTerm,
                                new IdentityTerm(identity.Identity));
                        }

                        context.WillApplyRightToLeft = terms[0].RightToLeft;
                    }
                    else
                    {
                        context.CurrentTerm = ParserUtilities.CombineTerms(
                            context.CurrentTerm,
                            new IdentityTerm(identity.Identity));
                    }
                    return ParseRunnerResult.Empty(this);

                case NumericToken numeric:
                    context.CurrentTerm = ParserUtilities.CombineTerms(
                        context.CurrentTerm,
                        ParserUtilities.GetNumericConstant(numeric.Value, Signes.Plus));
                    return ParseRunnerResult.Empty(
                        this);

                case NumericalSignToken numericSign:
                    // "abc -" / "123 -" ==> binary op or signed
                    if (context.LastToken is WhiteSpaceToken)
                    {
                        context.PreSignToken = numericSign;
                        return ParseRunnerResult.Empty(
                            NumericalSignedRunner.Instance);
                    }
                    // "abc-" / "123-" / "(abc)-" ==> binary op
                    else
                    {
                        context.CurrentTerm = ParserUtilities.CombineTerms(
                            context.CurrentTerm,
                            new IdentityTerm(numericSign.Symbol.ToString()));
                        return ParseRunnerResult.Empty(
                            this);
                    }

                case OpenParenthesisToken parenthesis:
                    context.Scopes.Push(
                        new ScopeInformation(context.CurrentTerm, parenthesis.Pair));
                    context.CurrentTerm = null;
                    return ParseRunnerResult.Empty(
                        WaitingRunner.Instance);

                case CloseParenthesisToken parenthesis:
                    if (ParserUtilities.LeaveScope(context, parenthesis.Pair))
                    {
                        return ParseRunnerResult.Empty(this);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Couldn't find open parenthesis: '{parenthesis.Pair.Open}'");
                    }

                default:
                    throw new InvalidOperationException(token.ToString());
            }
        }

        public static readonly ParseRunner Instance = new ApplyingRunner();
    }
}
