using Favalet.Contexts;
using Favalet.Expressions;
using Favalet.Expressions.Specialized;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Favalet
{
    // For testing purpose.
    public sealed class PseudoPlaceholderProvider
    {
        private sealed class Indexes
        {
            private readonly Dictionary<int, int> indexToPseudoIndex = new Dictionary<int, int>();
            private readonly Dictionary<int, int> pseudoIndexToIndex = new Dictionary<int, int>();

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

        private volatile int pseudoIndex = -1;

        private PseudoPlaceholderProvider()
        { }

        public ITerm CreatePlaceholder() =>
            new PseudoPlaceholderTerm(Interlocked.Increment(ref this.pseudoIndex));

        private static bool Equals(IExpression lhs, IExpression rhs, Indexes indexes)
        {
            if (object.ReferenceEquals(lhs, rhs))
            {
                return true;
            }

            if (lhs is PlaceholderTerm lp1 &&
                rhs is PseudoPlaceholderTerm rp1)
            {
                if (indexes.TryGetPseudoIndex(lp1.Index, out var rpi))
                {
                    return rpi == rp1.PseudoIndex;
                }
                else
                {
                    if (indexes.TryGetIndex(rp1.PseudoIndex, out var li))
                    {
                        return li == lp1.Index;
                    }
                    else
                    {
                        indexes.Set(lp1.Index, rp1.PseudoIndex);
                    }
                    return true;
                }
            }
            else if (lhs is PseudoPlaceholderTerm lp2 &&
                rhs is PlaceholderTerm rp2)
            {
                if (indexes.TryGetPseudoIndex(rp2.Index, out var lpi))
                {
                    return lpi == lp2.PseudoIndex;
                }
                else
                {
                    if (indexes.TryGetIndex(lp2.PseudoIndex, out var ri))
                    {
                        return ri == rp2.Index;
                    }
                    else
                    {
                        indexes.Set(rp2.Index, lp2.PseudoIndex);
                    }
                    return true;
                }
            }

            return (lhs, rhs) switch
            {
                (FourthTerm _, FourthTerm _) => true,
                (FourthTerm _, _) => false,
                (_, FourthTerm _) => false,
                (ILambdaExpression le, ILambdaExpression re) =>
                    Equals(le.Parameter, re.Parameter, indexes) &&
                    Equals(le.Body, re.Body, indexes),
                (IFunctionExpression le, IFunctionExpression re) =>
                    Equals(le.Parameter, re.Parameter, indexes) &&
                    Equals(le.Result, re.Result, indexes),
                (IApplyExpression le, IApplyExpression re) =>
                    Equals(le.Function, re.Function, indexes) &&
                    Equals(le.Argument, re.Argument, indexes),
                _ => lhs.Equals(rhs) && Equals(lhs.HigherOrder, rhs.HigherOrder, indexes)
            };
        }

        [DebuggerHidden]
        public bool Equals(IExpression lhs, IExpression rhs) =>
            Equals(lhs, rhs, new Indexes());

        public static PseudoPlaceholderProvider Create() =>
            new PseudoPlaceholderProvider();

        private sealed class PseudoPlaceholderTerm :
            Expression, ITerm
        {
            public readonly int PseudoIndex;

            public PseudoPlaceholderTerm(int pseudoIndex) =>
                this.PseudoIndex = pseudoIndex;

            public override IExpression HigherOrder =>
                throw new NotImplementedException();

            public override int GetHashCode() =>
                this.PseudoIndex.GetHashCode();

            public bool Equals(PseudoPlaceholderTerm rhs) =>
                throw new NotImplementedException();

            public override bool Equals(IExpression? other) =>
                throw new NotImplementedException();

            public override string GetPrettyString(PrettyStringContext context) =>
                $"'{this.PseudoIndex}";

            protected override IExpression Fixup(IReduceContext context) =>
                throw new NotImplementedException();

            protected override IExpression Infer(IReduceContext context) =>
                throw new NotImplementedException();

            protected override IExpression Reduce(IReduceContext context) =>
                throw new NotImplementedException();
        }
    }
}
