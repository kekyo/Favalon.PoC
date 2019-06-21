using Favalon.Expressions.Internals;
using System;

namespace Favalon.Expressions
{
    public sealed class TypeExpression : Expression
    {
        public readonly string Name;

        internal TypeExpression(string name) :
            base(KindExpression.Instance) =>
            this.Name = name;

        internal override bool CanProduceSafeReadableString =>
            true;
        internal override bool IsIgnoreAnnotationReadableString =>
            true;

        internal override string GetInternalReadableString(bool withAnnotation) =>
            this.Name.ToString();
    }
}
