﻿using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class KindExpression : IdentityExpression
    {
        private KindExpression() :
            base(UndefinedExpression.Instance, TextRange.Unknown)
        { }

        public override string Name =>
            "*";

        public override bool ShowInAnnotation =>
            false;

        protected internal override string FormatReadableString(FormatContext context) =>
            context.StrictNaming ? "(Kind)" : "*";

        public static readonly KindExpression Instance = new KindExpression();
    }
}
