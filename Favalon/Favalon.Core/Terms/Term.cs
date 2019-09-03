using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalon.Terms
{
    public abstract class Term : IEquatable<Term>
    {
        protected Term()
        {
        }

        public abstract bool Equals(Term? other);

        public override bool Equals(object? obj) =>
            this.Equals(obj as Term);
    }
}
