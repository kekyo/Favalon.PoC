using System;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Terms.Types
{
    internal static class ClrTypeCalculator
    {
        public static readonly IComparer<IClrTypeTerm> WideningComparer =
            new WideningComparerImpl();

        private sealed class WideningComparerImpl : IComparer<IClrTypeTerm>
        {
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

            public int Compare(IClrTypeTerm x, IClrTypeTerm y) =>
                (x, y) switch
                {
                    (Type tx, Type ty) => this.Compare(tx, ty),
                    //(ClrTypeTerm(_), _) => -1,
                    //(_, ClrTypeTerm(_)) => 1,
                    _ => 0
                };
        }
    }
}
