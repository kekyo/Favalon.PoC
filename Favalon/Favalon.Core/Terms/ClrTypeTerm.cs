using Favalon.Internal;
using System;

namespace Favalon.Terms
{
    public sealed class ClrTypeTerm :
        ValueTerm
    {
        public readonly Type ClrType;

        internal ClrTypeTerm(Type type) =>
            this.ClrType = type;

        public override object Constant =>
            this.ClrType;

        protected internal override string VisitTermString(bool includeTermName) =>
            this.ClrType.GetFullName();
    }
}
