using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Internals
{
    internal sealed class ParserCore
    {
        private readonly Stack<IVariableToken> applyingTokens =
            new Stack<IVariableToken>();

        private IEnumerable<Term> ExhaustTokens(Term finalTerm)
        {
            var term = finalTerm;
            Stack<Term>? terms = null;

            while (applyingTokens.Count >= 1)
            {
                var variableToken = applyingTokens.Pop();
                term = new ApplyTerm(variableToken.Value, term);
            }

            yield return term;

            while (terms?.Count >= 1)
            {
                yield return terms.Pop();
            }
        }

        private IEnumerable<Term> ExhaustTokens(Token finalToken) =>
            this.ExhaustTokens(finalToken switch
                {
                    NumericToken _ =>
                        (Term)new NumericTerm(finalToken.Value),
                    StringToken _ =>
                        (Term)new StringTerm(finalToken.Value),
                    _ =>
                        (Term)new VariableTerm(finalToken.Value)
                });

        public IEnumerable<Term> Examine(Token token)
        {
            switch (token)
            {
                case VariableToken variableToken when bool.TryParse(variableToken.Value, out var boolValue):
                    return this.ExhaustTokens(new BooleanTerm(boolValue));
                case IVariableToken variableToken:
                    applyingTokens.Push(variableToken);
                    return Enumerable.Empty<Term>();
                default:
                    return this.ExhaustTokens(token);
            }
        }

        public IEnumerable<Term> Flush() =>
            applyingTokens.Count switch
            {
                0 => Enumerable.Empty<Term>(),
                _ => this.ExhaustTokens((Token)applyingTokens.Pop())
            };
    }
}
