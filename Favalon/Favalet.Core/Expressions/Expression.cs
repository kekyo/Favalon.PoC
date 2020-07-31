using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System;
using System.Collections;
using System.Diagnostics;
using System.Xml.Linq;

namespace Favalet.Expressions
{
    public interface IExpression : IEquatable<IExpression?>
    {
        string Type { get; }

        IExpression HigherOrder { get; }
    }

    public interface ITerm : IExpression
    {
    }

    #pragma warning disable CS0659

    [DebuggerDisplay("{Readable}")]
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

        [DebuggerStepThrough]
        internal IExpression InternalInfer(IReduceContext context) =>
            this.Infer(context);
        [DebuggerStepThrough]
        internal IExpression InternalFixup(IReduceContext context) =>
            this.Fixup(context);
        [DebuggerStepThrough]
        internal IExpression InternalReduce(IReduceContext context) =>
            this.Reduce(context);

        protected abstract IEnumerable GetXmlValues(IXmlRenderContext context);

        protected abstract string GetPrettyString(IPrettyStringContext context);

        [DebuggerStepThrough]
        internal IEnumerable InternalGetXmlValues(IXmlRenderContext context) =>
            this.GetXmlValues(context);
        [DebuggerStepThrough]
        internal string InternalGetPrettyString(IPrettyStringContext context) =>
            this.GetPrettyString(context);

        public string Xml =>
            this.GetXml().ToString();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public string Readable =>
            this.GetPrettyString(PrettyStringTypes.Readable);

        public override sealed string ToString() =>
            this.GetPrettyString(PrettyStringTypes.Strict);

        public abstract bool Equals(IExpression? other);

        [DebuggerStepThrough]
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
                _ =>
                    lhs.Equals(rhs) &&
                    ExactEquals(lhs.HigherOrder, rhs.HigherOrder)
            };

        [DebuggerStepThrough]
        public static XElement GetXml(this IExpression expression) =>
            XmlRenderContext.Create().
            GetXml(expression);

        [DebuggerStepThrough]
        public static string GetPrettyString(this IExpression expression, PrettyStringTypes type) =>
            PrettyStringContext.Create(type).
            GetPrettyString(expression);
    }
}
