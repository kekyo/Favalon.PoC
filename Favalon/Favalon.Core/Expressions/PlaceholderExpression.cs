using System;

namespace Favalon.Expressions
{
    public sealed class PlaceholderExpression : Expression
    {
        public readonly int Index;

        internal PlaceholderExpression(int index) :
            base(UndefinedExpression.Instance) =>
            this.Index = index;

        public override string ReadableString
        {
            get
            {
                var ch = (char)('a' + (this.Index % ('z' - 'a' + 1)));
                var suffixIndex = this.Index / ('z' - 'a' + 1);
                var suffix = (suffixIndex >= 1) ? suffixIndex.ToString() : string.Empty;
                return $"'{ch}{suffix}";
            }
        }

        internal override Expression Visit(ExpressionEnvironment environment) =>
            this;

        internal override void Resolve(ExpressionEnvironment environment)
        {
        }
    }
}
