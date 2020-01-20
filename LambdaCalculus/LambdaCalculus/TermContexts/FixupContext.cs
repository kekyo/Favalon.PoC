using Favalon.Terms;
using System.Collections.Generic;

namespace Favalon.Contexts
{
    public class FixupContext : Context
    {
        private protected readonly Dictionary<int, Term> placeholders;

        private protected FixupContext(
            Context parent,
            Dictionary<string, Term> boundTerms,
            Dictionary<int, Term> placeholders) :
            base(parent, boundTerms) =>
            this.placeholders = placeholders;

        public Term? LookupUnifiedTerm(PlaceholderTerm placeholder)
        {
            var current = placeholder;
            Term? last = null;
            while (true)
            {
                if (placeholders.TryGetValue(current.Index, out var next))
                {
                    if (next is PlaceholderTerm p)
                    {
                        current = p;
                        last = p;
                    }
                    else
                    {
                        return next;
                    }
                }
                else
                {
                    return last;
                }
            }
        }
    }
}
