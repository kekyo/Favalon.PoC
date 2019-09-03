using Favalon.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalon
{
    public sealed class Inferrer
    {
        private readonly Dictionary<string, Term> variables = new Dictionary<string, Term>();

        public void AddVariable(string variable, Term term) =>
            variables.Add(variable, term);

        public IEnumerable<Term> Infer(IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                yield return token.TokenType switch
                {
                TokenTypes.Variable =>
                    variables.TryGetValue(token.Value, out var term) ?
                        term :
                        (Term)new VariableTerm(token.Value),
                TokenTypes.Numeric =>
                    (Term)new NumericTerm(token.Value),
                TokenTypes.String =>
                    (Term)new StringTerm(token.Value),
                    _ => throw new Exception()
                };
            }
        }
    }
}
