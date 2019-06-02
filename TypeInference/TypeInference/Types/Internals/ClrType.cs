using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TypeInferences.Types.Internals
{
    internal sealed class ClrType : AvalonType
    {
        private static readonly HashSet<(TypeInfo target, TypeInfo source)> convertibles =
            new HashSet<(TypeInfo target, TypeInfo source)>(new[] {
                (typeof(double), typeof(int)),
                (typeof(double), typeof(ushort)),
                (typeof(int), typeof(ushort)),
            }.
            Select(entry => (entry.Item1.GetTypeInfo(), entry.Item2.GetTypeInfo())));

        private readonly TypeInfo type;

        public ClrType(TypeInfo type) =>
            this.type = type;

        public override AvalonTypes Type =>
            AvalonTypes.ClrType;

        public override string Identity =>
            this.type.FullName;

        internal override bool IsConvertibleFrom(AvalonType rhs)
        {
            if (rhs is ClrType clrType)
            {
                if (this.type.IsAssignableFrom(clrType.type))
                {
                    return true;
                }
                if (convertibles.Contains((this.type, clrType.type)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
