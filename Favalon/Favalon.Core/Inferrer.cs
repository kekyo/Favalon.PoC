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
        public IEnumerable<Term> Infer(IEnumerable<Token> tokens)
        {
            foreach (var token in tokens)
            {
                yield return token.TokenType switch
                {
                TokenTypes.Variable =>
                    bool.TryParse(token.Value, out var boolValue) ?
                        (Term)new BooleanTerm(boolValue) :
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
