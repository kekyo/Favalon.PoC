using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public interface IExpression : IEquatable<IExpression?>
    {
        IExpression HigherOrder { get; }
    }

    public interface ITerm : IExpression
    {
    }

    #pragma warning disable CS0659

    [DebuggerDisplay("{DebugPrint}")]
    public abstract class Expression :
        IExpression
    {
        [DebuggerStepThrough]
        protected Expression()
        { }

        public string Type =>
            this.GetType().Name.
            Replace("Expression", string.Empty).
            Replace("Term", string.Empty);

        public abstract IExpression HigherOrder { get; }

        protected abstract IExpression Infer(IReduceContext context);
        protected abstract IExpression Fixup(IReduceContext context);
        protected abstract IExpression Reduce(IReduceContext context);

        [DebuggerHidden]
        internal IExpression InternalInfer(IReduceContext context) =>
            this.Infer(context);
        [DebuggerHidden]
        internal IExpression InternalFixup(IReduceContext context) =>
            this.Fixup(context);
        [DebuggerHidden]
        internal IExpression InternalReduce(IReduceContext context) =>
            this.Reduce(context);

        protected abstract string GetPrettyString(IPrettyStringContext context);

        [DebuggerHidden]
        internal string InternalGetPrettyString(IPrettyStringContext context) =>
            this.GetPrettyString(context);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebugPrint =>
            this.GetPrettyString(PrettyStringTypes.Readable);

        public string StrictAll =>
            this.GetPrettyString(PrettyStringTypes.StrictAll);

        public override sealed string ToString() =>
            this.GetPrettyString(PrettyStringTypes.Strict);

        public abstract bool Equals(IExpression? other);

        [DebuggerHidden]
        public override bool Equals(object obj) =>
            this.Equals(obj as IExpression);
    }

    public static class ExpressionExtension
    {
        public static bool ExactEquals(this IExpression lhs, IExpression rhs) =>
            object.ReferenceEquals(lhs, rhs) ||
            (lhs, rhs) switch
            {
                (FourthTerm _, FourthTerm _) => true,
                (FourthTerm _, _) => false,
                (_, FourthTerm _) => false,
                _ => lhs.Equals(rhs) && lhs.HigherOrder.ExactEquals(rhs.HigherOrder)
            };

        [DebuggerHidden]
        public static string GetPrettyString(this IExpression expression, PrettyStringTypes type) =>
            PrettyStringContext.Create(type).
            GetPrettyString(expression);
    }
}
