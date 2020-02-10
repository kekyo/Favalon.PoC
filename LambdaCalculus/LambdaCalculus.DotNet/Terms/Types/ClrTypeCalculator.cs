using Favalon.Terms.Methods;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Favalon.Terms.Types
{
    public sealed class ClrTypeCalculator : TypeCalculator
    {
        private static readonly Dictionary<Type, HashSet<Type>> wideningPrimitives = new Dictionary<Type, HashSet<Type>>
        {
            { typeof(decimal), new HashSet<Type> { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong), typeof(float), typeof(double), typeof(char) } },
            { typeof(double), new HashSet<Type> { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(float), typeof(char) } },
            { typeof(float), new HashSet<Type> { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(char) } },
            { typeof(long), new HashSet<Type> { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(char) } },
            { typeof(ulong), new HashSet<Type> { typeof(byte), typeof(ushort), typeof(uint), typeof(char) } },
            { typeof(int), new HashSet<Type> { typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(char) } },
            { typeof(uint), new HashSet<Type> { typeof(byte), typeof(ushort), typeof(char) } },
            { typeof(short), new HashSet<Type> { typeof(byte), typeof(sbyte) } },
            { typeof(ushort), new HashSet<Type> { typeof(byte), typeof(char) } },
            { typeof(char), new HashSet<Type> { typeof(byte), typeof(ushort) } },
        };

        private ClrTypeCalculator()
        { }

        private static bool IsAssignableFrom(Type to, Type from) =>
            to.IsAssignableFrom(from) ||
            (wideningPrimitives.TryGetValue(to, out var fromTypes) && fromTypes.Contains(from));

        public override Term? Widen(Term? to, Term? from)
        {
            switch (to, from)
            {
                // object: object <-- int
                // double: double <-- int
                // IComparable: IComparable <-- string
                case (ClrTypeTerm toType, ClrTypeTerm fromType):
                    return IsAssignableFrom(toType.Type, fromType.Type) ?
                        to :
                        null;

                case (ClrMethodTerm toMethod, ClrMethodTerm fromMethod):
                    return (this.Widen(toMethod.HigherOrder, fromMethod.HigherOrder) != null) ?
                        toMethod :
                        null;

                default:
                    return base.Widen(to, from);
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
