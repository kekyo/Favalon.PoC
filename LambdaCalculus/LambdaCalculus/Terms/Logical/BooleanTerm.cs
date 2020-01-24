using Favalon.Terms.Contexts;

namespace Favalon.Terms.Logical
{
    public abstract class BooleanTerm : IdentityTerm<BooleanTerm>
    {
        private protected BooleanTerm(string identity, Term higherOrder) :
            base(identity, higherOrder)
        { }

        public abstract bool Value { get; }

        protected override sealed bool IsIncludeHigherOrderInPrettyPrinting(HigherOrderDetails higherOrderDetail) =>
            false;

        public static readonly Term Type =
            TermFactory.Identity("bool");   // TODO: misunderstanding overrided bool terms...?

        public static BooleanTerm From(bool value) =>
            value ? True : False;

        public static BooleanTerm From(bool value, Term higherOrder) =>
            higherOrder.Equals(Type) ? From(value) :
                value ? (BooleanTerm)new TrueTerm(higherOrder) : new FalseTerm(higherOrder);

        public static readonly BooleanTerm True =
            new TrueTerm(Type);
        public static readonly BooleanTerm False =
            new FalseTerm(Type);

        public static implicit operator bool(BooleanTerm term) =>
            term.Value;
        public static bool operator !(BooleanTerm term) =>
            !term.Value;
    }

    public class TrueTerm : BooleanTerm
    {
        protected internal TrueTerm(Term higherOrder) :
            base("true", higherOrder)
        { }

        public override sealed bool Value =>
            true;

        protected override Term OnCreate(string identity, Term higherOrder) =>
            new TrueTerm(higherOrder);
    }

    public class FalseTerm : BooleanTerm
    {
        protected internal FalseTerm(Term higherOrder) :
            base("false", higherOrder)
        { }

        public override sealed bool Value =>
            false;

        protected override Term OnCreate(string identity, Term higherOrder) =>
            new FalseTerm(higherOrder);
    }
}
