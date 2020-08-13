using Favalet.Contexts;
using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Expressions.Specialized
{
    [DebuggerStepThrough]
    public sealed class UnspecifiedTerm :
        Expression, ITerm
    {
        private readonly PlaceholderOrderHints orderHint;

        private UnspecifiedTerm(PlaceholderOrderHints orderHint) =>
            this.orderHint = orderHint;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override IExpression HigherOrder =>
            DeadEndTerm.Instance;

        public bool Equals(UnspecifiedTerm rhs) =>
            rhs != null;

        public override bool Equals(IExpression? other) =>
            other is UnspecifiedTerm;

        protected override IExpression MakeRewritable(IMakeRewritableContext context) =>
            context is IPlaceholderProvider provider ?
                (IExpression)provider.CreatePlaceholder(this.orderHint) :
                this;

        protected override IExpression Infer(IReduceContext context) =>
            this;

        protected override IExpression Fixup(IFixupContext context) =>
            this;

        protected override IExpression Reduce(IReduceContext context) =>
            this;

        protected override IEnumerable GetXmlValues(IXmlRenderContext context) =>
            Enumerable.Empty<object>();

        protected override string GetPrettyString(IPrettyStringContext context) =>
            context.Type switch
            {
                PrettyStringTypes.Readable => "_",
                _ => this.orderHint.ToString()
            };

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm(PlaceholderOrderHints.VariableOrAbove);

        internal static readonly UnspecifiedTerm TypeInstance =
            new UnspecifiedTerm(PlaceholderOrderHints.TypeOrAbove);
    }
}
