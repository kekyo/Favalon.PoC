using Favalon.Terms;
using Favalon.Tokens;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Internals
{
    internal sealed class ParserCore
    {
        private readonly Dictionary<string, Term> variables =
            new Dictionary<string, Term>();
        private readonly Stack<IVariableToken> applyingTokens =
            new Stack<IVariableToken>();

        public void AddVariable(string variable, Term term) =>
            variables.Add(variable, term);

        private IEnumerable<Term> ExhaustTokens(Token finalToken)
        {
            var term = finalToken switch
            {
                NumericToken _ => (Term)new NumericTerm(finalToken.Value),
                StringToken _ => (Term)new StringTerm(finalToken.Value),
                _ => variables.TryGetValue(finalToken.Value, out var variable) ?
                    variable :
                    new VariableTerm(finalToken.Value)
            };

            Stack<Term>? terms = null;

            while (applyingTokens.Count >= 1)
            {
                var variableToken = applyingTokens.Pop();

                if (variables.TryGetValue(variableToken.Value, out var variable))
                {
                    if (terms == null)
                    {
                        terms = new Stack<Term>();
                    }
                    terms.Push(term);
                    term = variable;
                    continue;
                }
                else
                {
                    term = new ApplyTerm(variableToken.Value, term);
                }
            }

            yield return term;

            while (terms?.Count >= 1)
            {
                yield return terms.Pop();
            }
        }

        public IEnumerable<Term> Examine(Token token)
        {
            switch (token)
            {
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
