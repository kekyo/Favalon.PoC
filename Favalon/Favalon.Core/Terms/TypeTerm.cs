using Favalon.Internal;
using System;

namespace Favalon.Terms
{
    public sealed class TypeTerm :
        ValueTerm
    {
        public readonly Type Type;

        internal TypeTerm(Type type) =>
            this.Type = type;

        public override object Constant =>
            this.Type;

        protected internal override string VisitTermString(bool includeTermName) =>
            this.Type.GetFullName();
    }
}
