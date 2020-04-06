using Favalon.Terms.Contexts;

namespace Favalon.Terms.Types
{
    public sealed class KindTerm : IdentityTerm<KindTerm>
    {
        private KindTerm(string identity) :
            base(identity, TerminationTerm.Instance)
        { }

        protected override Term OnCreate(string identity, Term higherOrder) =>
            new KindTerm(identity);

        public override Term Reduce(ReduceContext context) =>
            this;

        protected override bool IsIncludeHigherOrderInPrettyPrinting(HigherOrderDetails higherOrderDetail) =>
            false;

        public static readonly KindTerm Instance =
            new KindTerm("*");

        public static KindTerm Create(string identity) =>
            new KindTerm(identity);
    }
}
