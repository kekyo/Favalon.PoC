using Favalet.Contexts;
using Favalet.Expressions;
using Favalet.Expressions.Specialized;
using System;

namespace Favalet
{
    // For testing purpose.
    public sealed class PseudoPlaceholderTerm :
        Expression, IPlacehoderTerm
    {
        public readonly int Index;

        private PseudoPlaceholderTerm(int index) =>
            this.Index = index;

        public override IExpression HigherOrder =>
            throw new NotImplementedException();

        int IPlacehoderTerm.Index =>
            this.Index;

        public override int GetHashCode() =>
            this.Index.GetHashCode();

        public bool Equals(IPlacehoderTerm rhs) =>
            this.Index == rhs.Index;

        public override bool Equals(IExpression? other) =>
            other is IPlacehoderTerm rhs && this.Equals(rhs);

        public override string GetPrettyString(PrettyStringContext context) =>
            $"'P{this.Index}";

        protected override IExpression Fixup(IReduceContext context) =>
            throw new NotImplementedException();

        protected override IExpression Infer(IReduceContext context) =>
            throw new NotImplementedException();

        protected override IExpression Reduce(IReduceContext context) =>
            throw new NotImplementedException();

        public static PseudoPlaceholderTerm Create(int index) =>
            new PseudoPlaceholderTerm(index);
    }
}
