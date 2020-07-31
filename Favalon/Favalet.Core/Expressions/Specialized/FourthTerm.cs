﻿using Favalet.Contexts;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Expressions.Specialized
{
    public sealed class FourthTerm :
        Expression, ITerm
    {
        private FourthTerm()
        { }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override IExpression HigherOrder =>
            TerminationTerm.Instance;

        public bool Equals(FourthTerm rhs) =>
            rhs != null;

        public override bool Equals(IExpression? other) =>
            other is FourthTerm;

        protected override IExpression Infer(IReduceContext context) =>
            this;

        protected override IExpression Fixup(IReduceContext context) =>
            this;

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        protected override IEnumerable GetXmlValues(IXmlRenderContext context) =>
            Enumerable.Empty<object>();

        protected override string GetPrettyString(IPrettyStringContext context) =>
            "#";

        public static readonly FourthTerm Instance =
            new FourthTerm();
    }
}
