using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalon.Terms
{
    partial class Term
    {
        public static VariableTerm Variable(string name) =>
            new VariableTerm(name);

        public static FunctionTerm Function(VariableTerm parameter, Term body) =>
            new FunctionTerm(parameter, body);

        public static ApplyTerm Apply(Term function, Term argument) =>
            new ApplyTerm(function, argument);
    }
}
