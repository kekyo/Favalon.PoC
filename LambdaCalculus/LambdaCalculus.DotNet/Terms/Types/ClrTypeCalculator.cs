using System;

namespace Favalon.Terms.Types
{
    internal sealed class ClrTypeCalculator : TypeCalculator
    {
        private ClrTypeCalculator()
        { }

        public override bool IsAssignable(Term toType, Term fromType) =>
            (toType, fromType) switch
            {
                (ClrTypeTerm to, ClrTypeTerm from) => to.Type.IsAssignableFrom(from.Type),
                _ => base.IsAssignable(toType, fromType)
            };

        private int Compare(Type x, Type y)
        {
            if (x.Equals(y))
            {
                return 0;
            }
            else if (x.IsPrimitive && !y.IsPrimitive)
            {
                return -1;
            }
            else if (!x.IsPrimitive && y.IsPrimitive)
            {
                return 1;
            }
            else if (x.IsPrimitive && y.IsPrimitive)
            {
                var cx = x.IsClsCompliant();
                var cy = y.IsClsCompliant();
                if (cx && !cy)
                {
                    return -1;
                }
                else if (!cx && cy)
                {
                    return -1;
                }

                var ix = x.IsInteger();
                var iy = y.IsInteger();
                if (ix && !iy)
                {
                    return -1;
                }
                else if (!ix && iy)
                {
                    return -1;
                }

                var sx = x.SizeOf();
                var sy = y.SizeOf();
                if (sx < sy)
                {
                    return -1;
                }
                else if (sx > sy)
                {
                    return 1;
                }
            }
            else if (x.IsValueType && !y.IsValueType)
            {
                return -1;
            }
            else if (!x.IsValueType && y.IsValueType)
            {
                return 1;
            }
            else if (y.IsAssignableFrom(x))
            {
                return -1;
            }
            else if (x.IsAssignableFrom(y))
            {
                return 1;
            }

            return -1;
        }

        public override int Compare(Term x, Term y) =>
            (x, y) switch
            {
                (ClrTypeTerm to, ClrTypeTerm from) => this.Compare(to.Type, from.Type),
                _ => base.Compare(x, y)
            };
    }
}
