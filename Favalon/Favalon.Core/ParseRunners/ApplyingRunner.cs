using Favalon.Terms;
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

            switch (token)
            {
                case IdentityToken identity:
                    if (context.Context.LookupBoundTerms(identity.Identity) is BoundTermInformation[] terms)
                    {
                        if (terms[0].Infix)
                        {
                            if (context.CurrentTerm is ApplyTerm(Term left, Term right))
                            {
                                context.CurrentTerm = CombineTerms(
                                    left,
                                    new IdentityTerm(identity.Identity),
                                    right);
                            }
                            else
                            {
                                context.CurrentTerm = CombineTerms(
                                    new IdentityTerm(identity.Identity),
                                    context.CurrentTerm);
                            }
                        }
                        else
                        {
                            context.CurrentTerm = CombineTerms(
                                context.CurrentTerm,
                                new IdentityTerm(identity.Identity));
                        }
   
                        if (terms[0].RightToLeft)
                        {
                            context.Scopes.Push(
                                new ScopeInformation(context.CurrentTerm));
                            context.CurrentTerm = null;
                            return ParseRunnerResult.Empty(
                                WaitingRunner.Instance);
                        }
                    }
                    else
                    {
                        context.CurrentTerm = CombineTerms(
                            context.CurrentTerm,
                            new IdentityTerm(identity.Identity));
                    }

                    return ParseRunnerResult.Empty(this);

                case OpenParenthesisToken parenthesis:
                    context.Scopes.Push(
                        new ScopeInformation(context.CurrentTerm, parenthesis.Pair));
                    context.CurrentTerm = null;
                    return ParseRunnerResult.Empty(
                        WaitingRunner.Instance);

                case CloseParenthesisToken parenthesis:
                    if (context.Scopes.Count == 0)
                    {
                        throw new InvalidOperationException(
                            $"Couldn't find open parenthesis: '{parenthesis.Pair.Open}'");
                    }
                    var parenthesisScope = context.Scopes.Pop();
                    if (!(parenthesisScope.ParenthesisPair?.Close == parenthesis.Pair.Close))
                    {
                        throw new InvalidOperationException(
                            $"Unmatched parenthesis: {parenthesis.Pair}");
                    }
                    context.CurrentTerm = CombineTerms(
                        parenthesisScope.SavedTerm,
                        context.CurrentTerm);
                    return ParseRunnerResult.Empty(
                        this);

                case NumericToken numeric:
                    context.CurrentTerm = CombineTerms(
                        context.CurrentTerm,
                        GetNumericConstant(numeric.Value, Signes.Plus));
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
                        context.CurrentTerm = CombineTerms(
                            context.CurrentTerm,
                            new IdentityTerm(numericSign.Symbol.ToString()));
                        return ParseRunnerResult.Empty(
                            this);
                    }

                case WhiteSpaceToken _:
                    return ParseRunnerResult.Empty(
                        this);

                default:
                    throw new InvalidOperationException();
            }
        }

        public static readonly ParseRunner Instance = new ApplyingRunner();
    }
}
