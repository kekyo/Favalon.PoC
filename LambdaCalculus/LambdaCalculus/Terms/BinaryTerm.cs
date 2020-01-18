using Favalon.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Terms
{
    public abstract class BinaryTerm : Term
    {
        public readonly Term Lhs;
        public readonly Term Rhs;

        protected BinaryTerm(Term lhs, Term rhs, Term higherOrder)
        {
            this.Lhs = lhs;
            this.Rhs = rhs;
            this.HigherOrder = higherOrder;
        }

        public override sealed Term HigherOrder { get; }

        protected abstract Term OnCreate(Term lhs, Term rhs, Term higherOrder);

        public override Term Infer(InferContext context)
        {
            var lhs = this.Lhs.Infer(context);
            var rhs = this.Rhs.Infer(context);
            var higherOrder = this.HigherOrder.Infer(context);

            context.Unify(lhs.HigherOrder, higherOrder);
            context.Unify(rhs.HigherOrder, higherOrder);

            return
                object.ReferenceEquals(lhs, this.Lhs) &&
                object.ReferenceEquals(rhs, this.Rhs) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                    this :
                    this.OnCreate(lhs, rhs, higherOrder);
        }

        public override Term Fixup(FixupContext context)
        {
            var lhs = this.Lhs.Fixup(context);
            var rhs = this.Rhs.Fixup(context);
            var higherOrder = this.HigherOrder.Fixup(context);

            return
                object.ReferenceEquals(lhs, this.Lhs) &&
                object.ReferenceEquals(rhs, this.Rhs) &&
                object.ReferenceEquals(higherOrder, this.HigherOrder) ?
                    this :
                    this.OnCreate(lhs, rhs, higherOrder);
        }

        public void Deconstruct(out Term lhs, out Term rhs, out Term higherOrder)
        {
            lhs = this.Lhs;
            rhs = this.Rhs;
            higherOrder = this.HigherOrder;
        }
    }

    public abstract class BinaryTerm<T> : BinaryTerm
        where T : BinaryTerm
    {
        protected BinaryTerm(Term lhs, Term rhs, Term higherOrder) :
            base(lhs, rhs, higherOrder)
        { }

        public override bool Equals(Term? other) =>
            object.ReferenceEquals(this, other) ||
                (other is T term ?
                    this.Lhs.Equals(term.Lhs) && this.Rhs.Equals(term.Rhs) :
                    false);

        public IEnumerable<Term> Flatten() =>
            (this.Lhs is BinaryTerm<T> lhs ?
                lhs.Flatten() :
                new[] { this.Lhs }).
            Concat(this.Rhs is BinaryTerm<T> rhs ?
                rhs.Flatten() :
                new[] { this.Rhs });

        public void Deconstruct(out Term[] terms) =>
            terms = this.Flatten().ToArray();
    }
}
