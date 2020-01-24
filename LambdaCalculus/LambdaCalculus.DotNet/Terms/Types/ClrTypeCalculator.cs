using Favalon.Terms.Algebraic;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Favalon.Terms.Types
{
    public sealed class ClrTypeCalculator : AlgebraicCalculator
    {
        private ClrTypeCalculator()
        { }

        protected override Term Sum(IEnumerable<Term> terms) =>
            ClrTermFactory.SumType(terms)!;

        public override Term? Widening(Term lhs, Term rhs)
        {
            switch ((lhs, rhs))
            {
                // object: object <-- int
                // IComparable: IComparable <-- string
                case (ClrTypeTerm to, ClrTypeTerm from):
                    return to.Type.IsAssignableFrom(from.Type) ? to : null;

                default:
                    return base.Widening(lhs, rhs);
            }
        }

        private int Compare(Type x, Type y)
        {
            if (x.Equals(y))
            {
                return 0;
            }
            else if (x.IsPrimitive() && !y.IsPrimitive())
            {
                return -1;
            }
            else if (!x.IsPrimitive() && y.IsPrimitive())
            {
                return 1;
            }
            else if (x.IsPrimitive() && y.IsPrimitive())
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
            else if (x.IsValueType() && !y.IsValueType())
            {
                return -1;
            }
            else if (!x.IsValueType() && y.IsValueType())
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

        public static readonly new ClrTypeCalculator Instance =
            new ClrTypeCalculator();
    }
}
