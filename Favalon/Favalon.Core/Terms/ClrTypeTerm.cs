using Favalon.Internal;
using System;
using System.Reflection;

namespace Favalon.Terms
{
    public sealed class ClrTypeTerm :
        ValueTerm
    {
#if NET35 || NET40 || NET45
        public readonly Type ClrType;

        internal ClrTypeTerm(Type type) =>
            this.ClrType = type;
#else
        public readonly TypeInfo ClrType;

        internal ClrTypeTerm(TypeInfo type) =>
            this.ClrType = type;
#endif
        public override object Constant =>
            this.ClrType;

        protected internal override string VisitTermString(bool includeTermName) =>
            this.ClrType.GetFullName();
    }
}
