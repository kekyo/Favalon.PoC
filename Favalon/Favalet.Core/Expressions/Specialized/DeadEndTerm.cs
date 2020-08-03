using Favalet.Contexts;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Expressions.Specialized
{
    internal interface IDeadEndTerm :
        ITerm
    {
    }

    [DebuggerStepThrough]
    internal sealed class DeadEndTerm :
        Expression, IDeadEndTerm
    {
        private DeadEndTerm()
        { }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override IExpression HigherOrder =>
            Instance;

        public bool Equals(FourthTerm rhs) =>
            false;

        public override bool Equals(IExpression? other) =>
            false;

        protected override IExpression Infer(IReduceContext context) =>
            this;

        protected override IExpression Fixup(IReduceContext context) =>
            this;

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        protected override IEnumerable GetXmlValues(IXmlRenderContext context) =>
            Enumerable.Empty<object>();

        protected override string GetPrettyString(IPrettyStringContext context) =>
            "#DeadEnd";

        public static readonly DeadEndTerm Instance =
            new DeadEndTerm();
    }
}
