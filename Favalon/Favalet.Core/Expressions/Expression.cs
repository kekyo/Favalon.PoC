using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public interface IExpression : IEquatable<IExpression?>
    {
        IExpression HigherOrder { get; }

        IExpression Infer(IReduceContext context);
        IExpression Fixup(IReduceContext context);
        IExpression Reduce(IReduceContext context);

        string GetPrettyString(PrettyStringContext type);
    }

    public interface ITerm : IExpression
    {
    }

    [DebuggerDisplay("{DebugPrint}")]
    public abstract class Expression :
        IExpression
    {
        protected Expression()
        { }

        public abstract IExpression HigherOrder { get; }

        public abstract IExpression Infer(IReduceContext context);
        public abstract IExpression Fixup(IReduceContext context);
        public abstract IExpression Reduce(IReduceContext context);

        public abstract string GetPrettyString(PrettyStringContext context);

        protected string FinalizePrettyString(PrettyStringContext context, string preFormatted)
        {
            var higherOrder = ((IExpression)this).HigherOrder;
            return (context.IsPartial, this, higherOrder) switch
            {
                (true, _, _) =>
                    preFormatted,
                (_, _, UnspecifiedTerm _) =>
                    preFormatted,
                (_, _, null) =>
                    preFormatted,
                (_, ITerm _, ITerm _) =>
                    $"{preFormatted}:{higherOrder.GetPrettyString(context.MakePartial())}",
                (_, ITerm _, _) =>
                    $"{preFormatted}:({higherOrder.GetPrettyString(context.MakePartial())})",
                (_, _, ITerm _) =>
                    $"({preFormatted}):{higherOrder.GetPrettyString(context.MakePartial())}",
                _ =>
                    $"({preFormatted}):({higherOrder.GetPrettyString(context.MakePartial())})",
            };
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebugPrint =>
            this.GetPrettyString(PrettyStringContext.Simple);

        public string StrictExpression =>
            this.GetPrettyString(PrettyStringContext.Strict);

        public override sealed string ToString() =>
            this.GetPrettyString(PrettyStringContext.Strict);

        public abstract bool Equals(IExpression? other);

        public override bool Equals(object obj) =>
            this.Equals(obj as IExpression);
    }
}
