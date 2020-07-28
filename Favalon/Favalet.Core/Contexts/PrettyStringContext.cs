using Favalet.Expressions;
using Favalet.Expressions.Specialized;

namespace Favalet.Contexts
{
    public struct PrettyStringContext
    {
        public readonly bool IsSimple;
        internal readonly bool IsPartial;

        public PrettyStringContext(bool isSimple)
        {
            IsSimple = isSimple;
            IsPartial = false;
        }

        private PrettyStringContext(bool isSimple, bool isPartial)
        {
            IsSimple = isSimple;
            IsPartial = isPartial;
        }

        private PrettyStringContext MakePartial() =>
            new PrettyStringContext(this.IsSimple, true);

        public string FinalizePrettyString(IExpression expression, string preFormatted)
        {
            var higherOrder = expression.HigherOrder;
            return (this.IsPartial, expression, higherOrder) switch
            {
                (true, _, _) =>
                    preFormatted,
                (_, _, UnspecifiedTerm _) =>
                    preFormatted,
                (_, _, null) =>
                    preFormatted,
                (_, ITerm _, ITerm _) =>
                    $"{preFormatted}:{higherOrder.GetPrettyString(this.MakePartial())}",
                (_, ITerm _, _) =>
                    $"{preFormatted}:({higherOrder.GetPrettyString(this.MakePartial())})",
                (_, _, ITerm _) =>
                    $"({preFormatted}):{higherOrder.GetPrettyString(this.MakePartial())}",
                _ =>
                    $"({preFormatted}):({higherOrder.GetPrettyString(this.MakePartial())})",
            };
        }

        public static readonly PrettyStringContext Simple =
            new PrettyStringContext(true);
        public static readonly PrettyStringContext Strict =
            new PrettyStringContext(false);
    }
}
