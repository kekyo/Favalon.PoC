using Favalon.ParseRunners;
using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;

namespace Favalon
{
    internal static class Parser
    {
        public static IEnumerable<Term> EnumerableTerms(
            Context context,
            IEnumerable<Token> tokens)
        {
            var runnerContext = ParseRunnerContext.Create(context);
            var runner = WaitingRunner.Instance;

            foreach (var token in tokens)
            {
                switch (runner.Run(runnerContext, token))
                {
                    case ParseRunnerResult(ParseRunner next, Term term):
                        yield return term;
                        runner = next;
                        break;
                    case ParseRunnerResult(ParseRunner next, _):
                        runner = next;
                        break;
                }

                runnerContext.LastToken = token;
            }

            if (runner.Finish(runnerContext) is ParseRunnerResult(_, Term finalTerm))
            {
                yield return finalTerm;
            }
        }



        //public static IEnumerable<Term> EnumerableTerms(
        //    Environment environment,
        //    IEnumerable<Token> tokens)
        //{
        //    ConstantTerm GetNumericConstant(string value, Signs preSign) =>
        //        new ConstantTerm(int.Parse(value, CultureInfo.InvariantCulture) * (int)preSign);

        //    Term TermOrApply(Term? left, Term? right) =>
        //        left is Term ?
        //            right is Term ?
        //                new ApplyTerm(left, right) :
        //                left :
        //            right!;

        //    Term? currentTerm = null;
        //    Token? lastToken = null;
        //    var stack = new Stack<OneOfTermInformation>();
        //    NumericalSignToken? preSign = null;
        //    var separatedSign = false;

        //    foreach (var token in tokens)
        //    {
        //        if (token is NumericalSignToken numericalSign)
        //        {
        //            preSign = numericalSign;
        //            separatedSign = (lastToken is WhiteSpaceToken) || (lastToken == null);
        //        }
        //        else
        //        {
        //            switch (token)
        //            {
        //                case WhiteSpaceToken _:
        //                    if (preSign is NumericalSignToken sign1)
        //                    {
        //                        currentTerm = TermOrApply(
        //                            currentTerm,
        //                            new IdentityTerm(sign1.ToString()));
        //                    }
        //                    break;

        //                case NumericToken numeric:
        //                    if (preSign is NumericalSignToken sign2)
        //                    {
        //                        // abc -123
        //                        if (separatedSign)
        //                        {
        //                            var numericTerm = GetNumericConstant(
        //                                numeric.Value,
        //                                sign2.Sign);
        //                            currentTerm = TermOrApply(
        //                                currentTerm,
        //                                numericTerm);
        //                        }
        //                        // abc-123
        //                        else
        //                        {
        //                            // Examined binary op
        //                            currentTerm = TermOrApply(
        //                                currentTerm,
        //                                new IdentityTerm(sign2.ToString()));
        //                            var numericTerm = GetNumericConstant(
        //                                numeric.Value,
        //                                Signs.Plus);
        //                            currentTerm = TermOrApply(
        //                                currentTerm,
        //                                numericTerm);
        //                        }
        //                    }
        //                    // abc 123
        //                    else
        //                    {
        //                        var numericTerm = GetNumericConstant(
        //                            numeric.Value,
        //                            Signs.Plus);
        //                        currentTerm = TermOrApply(
        //                            currentTerm,
        //                            numericTerm);
        //                    }
        //                    break;

        //                case IdentityToken identity:
        //                    currentTerm = TermOrApply(
        //                        currentTerm,
        //                        new IdentityTerm(identity.Identity));
        //                    break;

        //                case OpenParenthesisToken parenthesis:
        //                    stack.Push(new OneOfTermInformation(
        //                        currentTerm,
        //                        Characters.IsOpenParenthesis(parenthesis.Symbol)!.Value.Close));
        //                    currentTerm = null;
        //                    break;

        //                case CloseParenthesisToken parenthesis:
        //                    if (stack.Count >= 1)
        //                    {
        //                        var oneOfTermInformation = stack.Pop();
        //                        if (oneOfTermInformation.Close == parenthesis.Symbol)
        //                        {
        //                            if (oneOfTermInformation.Term != null)
        //                            {
        //                                currentTerm = TermOrApply(
        //                                    oneOfTermInformation.Term,
        //                                    currentTerm);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            // Invalid parenthesis pair.
        //                            throw new InvalidOperationException();
        //                        }
        //                    }
        //                    else
        //                    {
        //                        // Lack for open parenthesis.
        //                        throw new InvalidOperationException();
        //                    }
        //                    break;

        //                default:
        //                    throw new InvalidOperationException();
        //            }

        //            preSign = null;
        //            separatedSign = false;
        //        }

        //        lastToken = token;
        //    }

        //    if (stack.Count >= 1)
        //    {
        //        // Lack for close parenthesis.
        //        throw new InvalidOperationException();
        //    }

        //    if (currentTerm != null)
        //    {
        //        yield return currentTerm;
        //    }
        //}
    }
}
