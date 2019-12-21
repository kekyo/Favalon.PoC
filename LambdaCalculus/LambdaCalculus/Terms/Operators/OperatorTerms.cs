using Favalon.Contexts;

namespace Favalon.Terms.Operators
{
    public abstract class OperatorSymbolTerm : Term
    {
        internal OperatorSymbolTerm()
        { }

        public override Term HigherOrder =>
            UnspecifiedTerm.Instance;

        public override sealed Term Infer(InferContext context, Term higherOrderHint) =>
            this;

        public override sealed Term Fixup(FixupContext context) =>
            this;

        public override sealed Term Reduce(ReduceContext context) =>
            this;
    }

    public abstract class OperatorSymbolTerm<T> : OperatorSymbolTerm
        where T : Term
    {
        private static readonly int hashCode =
            typeof(T).GetHashCode();

        protected OperatorSymbolTerm()
        { }

        public override sealed bool Equals(Term? other) =>
            other is T;

        public override int GetHashCode() =>
            hashCode;
    }

    public abstract class OperatorArgument0Term : Term
    {
        public readonly Term Argument0;

        internal OperatorArgument0Term(Term argument0) =>
            this.Argument0 = argument0;

        public override Term HigherOrder =>
            UnspecifiedTerm.Instance;

        protected abstract Term Create(Term argument0);

        public override sealed Term Reduce(ReduceContext context) =>
            this;

        public override sealed Term Infer(InferContext context, Term higherOrderHint) =>
            this.Create(this.Argument0.Infer(context, UnspecifiedTerm.Instance));

        public override sealed Term Fixup(FixupContext context) =>
            this.Create(this.Argument0.Fixup(context));

        public override int GetHashCode() =>
            this.Argument0.GetHashCode();
    }

    public abstract class OperatorArgument0Term<T> : OperatorArgument0Term
        where T : OperatorArgument0Term
    {
        protected OperatorArgument0Term(Term argument) :
            base(argument)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T rhs ? this.Argument0.Equals(rhs.Argument0) : false;
    }

    public abstract class OperatorArgument1Term : Term
    {
        public readonly Term Argument0;
        public readonly Term Argument1;

        internal OperatorArgument1Term(Term argument0, Term argument1)
        {
            this.Argument0 = argument0;
            this.Argument1 = argument1;
        }

        public override Term HigherOrder =>
            UnspecifiedTerm.Instance;

        protected abstract Term Create(Term argument0, Term argument1);

        public override sealed Term Infer(InferContext context, Term higherOrderHint) =>
            this.Create(this.Argument0.Infer(context, UnspecifiedTerm.Instance), this.Argument1.Infer(context, UnspecifiedTerm.Instance));

        public override sealed Term Fixup(FixupContext context) =>
            this.Create(this.Argument0.Fixup(context), this.Argument1.Fixup(context));

        public override sealed Term Reduce(ReduceContext context) =>
            this;

        public override int GetHashCode() =>
            this.Argument0.GetHashCode() ^ this.Argument1.GetHashCode();
    }

    public abstract class OperatorArgument1Term<T> : OperatorArgument1Term
        where T : OperatorArgument1Term
    {
        protected OperatorArgument1Term(Term argument0, Term argument1) :
            base(argument0, argument1)
        { }

        public override sealed bool Equals(Term? other) =>
            other is T rhs ?
                (this.Argument0.Equals(rhs.Argument0) && this.Argument1.Equals(rhs.Argument1)) :
                false;
    }
}
