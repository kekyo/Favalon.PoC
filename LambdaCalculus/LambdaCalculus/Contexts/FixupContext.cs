using Favalon.Terms;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Contexts
{
    public class FixupContext : Context
    {
        private protected readonly Dictionary<int, Term> placeholders;

        private protected FixupContext(Context parent, Dictionary<int, Term> placeholders) :
            base(parent) =>
            this.placeholders = placeholders;

        public Term LookupUnifiedTerm(PlaceholderTerm placeholder)
        {
            var current = placeholder;
            while (true)
            {
                if (placeholders.TryGetValue(current.Index, out var next))
                {
                    if (next is PlaceholderTerm p)
                    {
                        current = p;
                    }
                    else
                    {
                        return next;
                    }
                }
                else
                {
                    return current;
                }
            }
        }
    }
}
