using Favalon.Terms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Favalon
{
    public static class Parser
    {
        public static Term? Parse(IEnumerable<Term> terms)
        {
            var enumerator = terms.GetEnumerator();
            try
            {
                if (enumerator.MoveNext())
                {
                    var term = enumerator.Current;
                    while (enumerator.MoveNext())
                    {
                        term = Factories.Apply(term, enumerator.Current);
                    }

                    return term;
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                if (enumerator is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
