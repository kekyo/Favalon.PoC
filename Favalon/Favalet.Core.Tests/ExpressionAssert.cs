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
            private readonly Dictionary<string, string> indexToPseudoIndex = new Dictionary<string, string>();
            private readonly Dictionary<string, string> pseudoIndexToIndex = new Dictionary<string, string>();

            [DebuggerStepThrough]
            public Indexes()
            { }

            public bool TryGetPseudoIndex(string index, out string pseudoIndex) =>
                this.indexToPseudoIndex.TryGetValue(index, out pseudoIndex);

            public bool TryGetIndex(string pseudoIndex, out string index) =>
                this.pseudoIndexToIndex.TryGetValue(pseudoIndex, out index);

            public void Set(string index, string pseudoIndex)
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

            if (lhs is IIdentityTerm lp1 &&
                rhs is PseudoPlaceholderProvider.PseudoPlaceholderTerm rp1)
            {
                if (indexes.TryGetPseudoIndex(lp1.Symbol, out var rpi))
                {
                    return Trap(rpi == rp1.Symbol);
                }
                else
                {
                    if (indexes.TryGetIndex(rp1.Symbol, out var li))
                    {
                        return Trap(li == lp1.Symbol);
                    }
                    else
                    {
                        indexes.Set(lp1.Symbol, rp1.Symbol);
                    }
                    return true;
                }
            }
            else if (lhs is PseudoPlaceholderProvider.PseudoPlaceholderTerm lp2 &&
                rhs is IIdentityTerm rp2)
            {
                if (indexes.TryGetPseudoIndex(rp2.Symbol, out var lpi))
                {
                    return Trap(lpi == lp2.Symbol);
                }
                else
                {
                    if (indexes.TryGetIndex(lp2.Symbol, out var ri))
                    {
                        return Trap(ri == rp2.Symbol);
                    }
                    else
                    {
                        indexes.Set(rp2.Symbol, lp2.Symbol);
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
                case (TerminationTerm _, _):
                    return Trap(false);
                case (_, TerminationTerm _):
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

        [DebuggerStepThrough]
        public static bool Equals(IExpression expected, IExpression actual) =>
            Equals(expected, actual, new Indexes());
    }
}
