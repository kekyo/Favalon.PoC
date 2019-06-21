﻿using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class KindExpression : Expression
    {
        private KindExpression() :
            base(UndefinedExpression.Instance)
        { }

        internal override bool CanProduceSafeReadableString =>
            true;
        internal override bool IsIgnoreAnnotationReadableString =>
            true;
        internal override bool IsIgnoreReadableString =>
            true;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            "(Kind)";

        internal override Expression Visit(ExpressionEnvironment environment, InferContext context) =>
            this;

        internal override Expression FixupChildren(InferContext context) =>
            this;

        internal static readonly KindExpression Instance = new KindExpression();
    }
}
