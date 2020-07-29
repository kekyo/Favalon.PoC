using Favalet.Expressions;
using Favalet.Expressions.Specialized;
using System.Collections.Generic;
using System.Diagnostics;

namespace Favalet
{
    internal static class ExpressionAssert
    {
        private sealed class Indexes
        {
            private readonly Dictionary<int, int> indexToPseudoIndex = new Dictionary<int, int>();
            private readonly Dictionary<int, int> pseudoIndexToIndex = new Dictionary<int, int>();

            [DebuggerStepThrough]
            public Indexes()
            { }

            public bool TryGetPseudoIndex(int index, out int pseudoIndex) =>
                this.indexToPseudoIndex.TryGetValue(index, out pseudoIndex);

            public bool TryGetIndex(int pseudoIndex, out int index) =>
                this.pseudoIndexToIndex.TryGetValue(pseudoIndex, out index);

            public void Set(int index, int pseudoIndex)
            {
                this.indexToPseudoIndex.Add(index, pseudoIndex);
                this.pseudoIndexToIndex.Add(pseudoIndex, index);
            }
        }

        private static bool Trap(bool result)
        {
            Trace.Assert(result || !Debugger.IsAttached);
            return result;
        }

        private static bool Equals(IExpression lhs, IExpression rhs, Indexes indexes)
        {
            if (object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (lhs is PlaceholderTerm lp1 &&
                rhs is PseudoPlaceholderProvider.PseudoPlaceholderTerm rp1)
            {
                if (indexes.TryGetPseudoIndex(lp1.Index, out var rpi))
                {
                    return Trap(rpi == rp1.PseudoIndex);
                }
                else
                {
                    if (indexes.TryGetIndex(rp1.PseudoIndex, out var li))
                    {
                        return Trap(li == lp1.Index);
                    }
                    else
                    {
                        indexes.Set(lp1.Index, rp1.PseudoIndex);
                    }
                    return true;
                }
            }
            else if (lhs is PseudoPlaceholderProvider.PseudoPlaceholderTerm lp2 &&
                rhs is PlaceholderTerm rp2)
            {
                if (indexes.TryGetPseudoIndex(rp2.Index, out var lpi))
                {
                    return Trap(lpi == lp2.PseudoIndex);
                }
                else
                {
                    if (indexes.TryGetIndex(lp2.PseudoIndex, out var ri))
                    {
                        return Trap(ri == rp2.Index);
                    }
                    else
                    {
                        indexes.Set(rp2.Index, lp2.PseudoIndex);
                    }
                    return true;
                }
            }

            switch (lhs, rhs)
            {
                case (UnspecifiedTerm _, _):   // Only expected expression
                    return true;
                case (FourthTerm _, FourthTerm _):
                    return true;
                case (FourthTerm _, _):
                    return Trap(false);
                case (_, FourthTerm _):
                    return Trap(false);
                case (ILambdaExpression le, ILambdaExpression re):
                    return
                        Equals(le.Parameter, re.Parameter, indexes) &&
                        Equals(le.Body, re.Body, indexes) &&
                        Equals(lhs.HigherOrder, rhs.HigherOrder, indexes);
                case (IFunctionExpression le, IFunctionExpression re):
                    return
                        Equals(le.Parameter, re.Parameter, indexes) &&
                        Equals(le.Result, re.Result, indexes) &&
                        Equals(lhs.HigherOrder, rhs.HigherOrder, indexes);
                case (IApplyExpression le, IApplyExpression re):
                    return
                        Equals(le.Function, re.Function, indexes) &&
                        Equals(le.Argument, re.Argument, indexes) &&
                        Equals(lhs.HigherOrder, rhs.HigherOrder, indexes);
                default:
                    return
                        Trap(lhs.Equals(rhs)) &&
                        Equals(lhs.HigherOrder, rhs.HigherOrder, indexes);
            }
        }

        [DebuggerHidden]
        public static bool Equals(IExpression expected, IExpression actual) =>
            Equals(expected, actual, new Indexes());
    }
}
