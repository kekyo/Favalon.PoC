using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class PlaceholderExpression : IdentityExpression
    {
        internal PlaceholderExpression(int index) :
            base(UndefinedExpression.Instance) =>
            this.Index = index;

        public int Index { get; internal set; }

        public override string Name
        {
            get
            {
                var ch = (char)('a' + (this.Index % ('z' - 'a' + 1)));
                var suffixIndex = this.Index / ('z' - 'a' + 1);
                var suffix = (suffixIndex >= 1) ? suffixIndex.ToString() : string.Empty;
                return $"'{ch}{suffix}";
            }
        }
    }
}
