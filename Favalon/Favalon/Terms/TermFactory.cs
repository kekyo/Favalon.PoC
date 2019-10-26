using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalon.Terms
{
    partial class Term
    {
        public static IdentityTerm Identity(string name) =>
            new IdentityTerm(name);

        public static FunctionTerm Function(IdentityTerm parameter, Term body) =>
            new FunctionTerm(parameter, body);

        public static ApplyTerm Apply(Term function, Term argument) =>
            new ApplyTerm(function, argument);
    }
}
