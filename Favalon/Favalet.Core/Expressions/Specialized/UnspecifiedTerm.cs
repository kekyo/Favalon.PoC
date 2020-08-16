﻿using Favalet.Contexts;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Expressions.Specialized
{
    [DebuggerStepThrough]
    public sealed class UnspecifiedTerm :
        Expression, ITerm
    {
        private UnspecifiedTerm()
        {
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override IExpression HigherOrder =>
            DeadEndTerm.Instance;

        public bool Equals(UnspecifiedTerm rhs) =>
            rhs != null;

        public override bool Equals(IExpression? other) =>
            other is UnspecifiedTerm;

        protected override IExpression MakeRewritable(IMakeRewritableContext context) =>
            context.CreatePlaceholder(context.OrderHint);

        protected override IExpression Infer(IInferContext context) =>
            this;

        protected override IExpression Fixup(IFixupContext context) =>
            this;

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        protected override IEnumerable GetXmlValues(IXmlRenderContext context) =>
            Enumerable.Empty<object>();

        protected override string GetPrettyString(IPrettyStringContext context) =>
            "_";

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
    }
}
