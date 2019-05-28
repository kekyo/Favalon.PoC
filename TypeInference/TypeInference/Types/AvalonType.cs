using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TypeInferences.Types
{
    public abstract partial class AvalonType
    {
        private protected AvalonType()
        {
        }

        public bool Equals(AvalonType rhs) =>
            object.ReferenceEquals(this, rhs);

        public virtual IEnumerable<AvalonType> EnumerateTypes() =>
            new[] { this };

        private protected virtual bool IsConvertibleFrom(AvalonType rhs) =>
            false;

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

        public static AvalonType Wide(params AvalonType[] types)
        {
            var rcTypes = new List<AvalonType>();

            foreach (var type in types)
            {
                var found = false;
                for (var index = 0; index < rcTypes.Count; index++)
                {
                    var rcType = rcTypes[index];
                    if (WideCore(rcType, type) is AvalonType calculated)
                    {
                        rcTypes[index] = calculated;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    rcTypes.Add(type);
                }
            }

            return (rcTypes.Count <= 1) ?
                rcTypes[0] :
                new UnionType(rcTypes.ToArray());
        }
    }
}
