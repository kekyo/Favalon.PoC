using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeInferences.Types.Internals;

namespace TypeInferences.Types
{
    public enum AvalonTypes
    {
        Unspecified,
        ClrType,
        Union
    }

    public abstract class AvalonType :
        IEquatable<AvalonType>, IComparable<AvalonType>
    {
        private protected AvalonType()
        {
        }

        public abstract AvalonTypes Type { get; }

        public abstract string Identity { get; }

        public override int GetHashCode() =>
            this.Identity.GetHashCode();

        public virtual bool Equals(AvalonType rhs) =>
            this.Identity == rhs.Identity;

        public virtual int CompareTo(AvalonType other) =>
            this.Identity.CompareTo(other.Identity);

        internal abstract bool IsConvertibleFrom(AvalonType rhs);

        internal AvalonType Normalized =>
            this is AvalonTypeRef typeRef ? typeRef.type : this;

        public override string ToString() =>
            this.Identity;

        private static AvalonType? WideCore(AvalonType lhs, AvalonType rhs)
        {
            if (object.ReferenceEquals(lhs, rhs))
            {
                return lhs;
            }
            else if (lhs.IsConvertibleFrom(rhs))
            {
                return lhs;
            }
            else if (rhs.IsConvertibleFrom(lhs))
            {
                return rhs;
            }
            else
            {
                return null;
            }
        }

        public AvalonType ToWide(params AvalonType[] types)
        {
            var rcTypes = new List<AvalonType> { this.Normalized };

            foreach (var type in types)
            {
                var normalizedType = type.Normalized;
                var found = false;
                for (var index = 0; index < rcTypes.Count; index++)
                {
                    var rcType = rcTypes[index];
                    if (WideCore(rcType, normalizedType) is AvalonType calculated)
                    {
                        rcTypes[index] = calculated;
                        found = true;
                    }
                }
                if (!found)
                {
                    rcTypes.Add(normalizedType);
                }
            }

            return (rcTypes.Count <= 1) ?
                rcTypes[0] :
                new UnionType(rcTypes.Distinct().ToArray());
        }

        public AvalonTypeRef MakeTypeRef() =>
            new AvalonTypeRef(this.Normalized);

        public static AvalonType FromClrType<T>() =>
            FromClrType(typeof(T).GetTypeInfo());

        public static AvalonType FromClrType(Type type) =>
            FromClrType(type.GetTypeInfo());

        public static AvalonType FromClrType(TypeInfo type) =>
            new ClrType(type);

        public static readonly AvalonType Unspecified =
            new UnspecifiedType();
    }
}
