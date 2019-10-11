using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public sealed class Environment
    {
        internal Environment()
        {
        }

        public Term Infer(Term term)
        {
            var inferred = term.VisitInfer(this);
            return inferred;
        }

        public static Environment Create() =>
            new Environment();
    }
}
