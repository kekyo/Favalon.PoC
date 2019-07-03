using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class PlaceholderExpression : VariableExpression, IEquatable<PlaceholderExpression?>, IComparable<PlaceholderExpression>
    {
        public readonly int Index;

        internal PlaceholderExpression(int index, TermExpression higherOrder) :
            base(higherOrder) =>
            this.Index = index;

        protected override (string formatted, bool requiredParentheses) FormatReadableString(FormatContext context)
        {
            if (context.StrictNaming)
            {
                return (this.Name, false);
            }
            else
            {
                var index = context.GetAdjustedIndex(this);

                var ch = (char)('a' + (index % ('z' - 'a' + 1)));
                var suffixIndex = index / ('z' - 'a' + 1);
                var suffix = (suffixIndex >= 1) ? suffixIndex.ToString() : string.Empty;
                return ($"'{ch}{suffix}", false);
            }
        }

        public override string Name =>
            $"'{this.Index}";

        protected override Expression VisitInferring(IInferringEnvironment environment, InferContext context)
        {
            var higherOrder = VisitInferring(environment, this.HigherOrder, context);
            return new PlaceholderExpression(this.Index, higherOrder);
        }

        protected override (bool isResolved, Expression resolved) VisitResolving(IResolvingEnvironment environment, InferContext context)
        {
            if (environment.GetRelatedExpression(context, this) is TermExpression related)
            {
                return (true, related);
            }
            else
            {
                var (rho, higherOrder) = VisitResolving(environment, this.HigherOrder, context);
                return rho ? (true, new PlaceholderExpression(this.Index, higherOrder)) : (false, this);
            }
        }

        public override int GetHashCode() =>
            this.Index.GetHashCode();

        public bool Equals(PlaceholderExpression? other) =>
            (other == null) ? false :
            object.ReferenceEquals(this, other) ? true :
            this.Index.Equals(other.Index);

        public override bool Equals(VariableExpression? other) =>
            this.Equals(other as PlaceholderExpression);

        public override bool Equals(object obj) =>
            this.Equals(obj as PlaceholderExpression);

        public int CompareTo(PlaceholderExpression other) =>
            this.Index.CompareTo(other.Index);
    }
}
