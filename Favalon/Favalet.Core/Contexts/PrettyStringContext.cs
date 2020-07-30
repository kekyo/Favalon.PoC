using Favalet.Expressions;
using Favalet.Expressions.Specialized;
using System.Diagnostics;

namespace Favalet.Contexts
{
    public interface IPrettyStringContext
    {
        string GetPrettyString(IExpression expression);
        string FinalizePrettyString(IExpression expression, string preFormatted);
    }

    public enum PrettyStringTypes
    {
        Readable,
        Strict,
        StrictAll
    }

    public sealed class PrettyStringContext :
        IPrettyStringContext
    {
        private readonly PrettyStringTypes type;
        private readonly bool isPartial;

        [DebuggerStepThrough]
        private PrettyStringContext(PrettyStringTypes type, bool isPartial)
        {
            this.type = type;
            this.isPartial = isPartial;
        }

        public string GetPrettyString(IExpression expression) =>
            (this.type, expression, expression) switch
            {
                (PrettyStringTypes.Readable, Expression expr, _) => expr.InternalGetPrettyString(this),
                (_, Expression expr, TerminationTerm _) => expr.InternalGetPrettyString(this),
                (_, Expression expr, _) => $"{expr.Type} {expr.InternalGetPrettyString(this)}",
                _ => this.FinalizePrettyString(expression, "?")
            };

        string IPrettyStringContext.GetPrettyString(IExpression expression) =>
            (this.type, expression, expression) switch
            {
                (PrettyStringTypes.Readable, Expression expr, ITerm _) => expr.InternalGetPrettyString(this),
                (PrettyStringTypes.Readable, Expression expr, _) => $"({expr.InternalGetPrettyString(this)})",
                (_, Expression expr, TerminationTerm _) => expr.InternalGetPrettyString(this),
                (_, Expression expr, _) => $"({expr.Type} {expr.InternalGetPrettyString(this)})",
                _ => this.FinalizePrettyString(expression, "?")
            };

        [DebuggerStepThrough]
        private IPrettyStringContext MakePartial() =>
            (this.type, this.isPartial) switch
            {
                (PrettyStringTypes.StrictAll, _) => this,
                _ => new PrettyStringContext(this.type, true),
            };

        public string FinalizePrettyString(IExpression expression, string preFormatted)
        {
            var higherOrder = expression.HigherOrder;
            return (this.type, this.isPartial, expression, higherOrder) switch
            {
                (_, true, _, _) =>
                    preFormatted,
                (_, _, _, TerminationTerm _) =>
                    preFormatted,
                (PrettyStringTypes.Readable, _, _, UnspecifiedTerm _) =>
                    preFormatted,
                (PrettyStringTypes.Readable, _, _, FourthTerm _) =>
                    preFormatted,
                (_, _, ITerm _, _) =>
                    $"{preFormatted}:{this.MakePartial().GetPrettyString(higherOrder)}",
                _ =>
                    $"({preFormatted}):{this.MakePartial().GetPrettyString(higherOrder)}",
            };
        }

        [DebuggerStepThrough]
        public static PrettyStringContext Create(PrettyStringTypes type) =>
            new PrettyStringContext(type, false);
    }
}
