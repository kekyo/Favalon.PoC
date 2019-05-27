using System;
using System.Reflection;

namespace TypeInference.Types
{
    public sealed class CLRType : AvalonType
    {
        private readonly TypeInfo typeInfo;

        internal CLRType(TypeInfo typeInfo) => this.typeInfo = typeInfo;

        public TypeInfo RawType => this.typeInfo;

        public override int GetHashCode() =>
            this.typeInfo.GetHashCode();

        public override bool Equals(AvalonType rhs) =>
            this.typeInfo.Equals((rhs as CLRType)?.typeInfo);

        public override ComparedResult CompareTo(AvalonType rhs)
        {
            var clrType = rhs as CLRType;
            if (clrType != null)
            {
                if (this.typeInfo.Equals(clrType.typeInfo))
                {
                    return ComparedResult.Equal;
                }

                if (this.typeInfo.IsAssignableFrom(clrType.typeInfo))
                {
                    return ComparedResult.NarrowThan;
                }
                else if (clrType.typeInfo.IsAssignableFrom(this.typeInfo))
                {
                    return ComparedResult.WideThan;
                }
            }

            return ComparedResult.Invalid;
        }
    }
}
