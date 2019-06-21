using Favalon.Expressions.Internals;
using System;

namespace Favalon.Expressions
{
    public sealed class IntegerExpression : Expression
    {
        public readonly int Value;

        internal IntegerExpression(int value) :
            base(Int32Type) =>
            this.Value = value;

        internal override bool CanProduceSafeReadableString =>
            true;
        internal override bool IsIgnoreAnnotationReadableString =>
            true;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            this.Value.ToString();

        internal override Expression Visit(ExpressionEnvironment environment, InferContext context) =>
            this;

        internal override Expression FixupChildren(InferContext context) =>
            this;

        private static readonly TypeExpression Int32Type = new TypeExpression("System.Int32");
    }
}
