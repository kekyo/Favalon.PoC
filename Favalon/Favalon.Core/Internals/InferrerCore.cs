using Favalon.Terms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalon.Internals
{
    internal sealed class InferrerCore
    {
        private readonly Dictionary<string, Term> variables =
            new Dictionary<string, Term>();
        private readonly Stack<Token> tokens = new Stack<Token>();

        public void AddVariable(string variable, Term term) =>
            variables.Add(variable, term);

        private IEnumerable<Term> ExhaustTokens(Token finalToken)
        {
            var term = finalToken.TokenType switch
            {
                TokenTypes.Numeric => (Term)new NumericTerm(finalToken.Value),
                TokenTypes.String => (Term)new StringTerm(finalToken.Value),
                _ => variables.TryGetValue(finalToken.Value, out var variable) ?
                    variable :
                    new VariableTerm(finalToken.Value)
            };

            Stack<Term>? terms = null;

            while (tokens.Count >= 1)
            {
                var token = tokens.Pop();
                Debug.Assert(token.TokenType == TokenTypes.Variable);

                if (variables.TryGetValue(token.Value, out var variable))
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
                    term = new ApplyTerm(token.Value, term);
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
            switch (token.TokenType)
            {
                case TokenTypes.Numeric:
                case TokenTypes.String:
                    return this.ExhaustTokens(token);
                default:
                    tokens.Push(token);
                    return Enumerable.Empty<Term>();
            }
        }

        public IEnumerable<Term> Flush() =>
            tokens.Count switch
            {
                0 => Enumerable.Empty<Term>(),
                _ => this.ExhaustTokens(tokens.Pop())
            };
    }
}
