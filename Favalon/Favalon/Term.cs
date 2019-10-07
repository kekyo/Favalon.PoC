using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon
{
    public abstract class Term
    {
        protected Term()
        { }

        public Term? HigherOrder { get; internal set; }
    }
}
