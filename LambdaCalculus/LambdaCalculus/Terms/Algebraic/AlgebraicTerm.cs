using Favalon.Terms.Contexts;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Terms.Algebraic
{
    public abstract class AlgebraicTerm : Term
    {
        public readonly Term[] Terms;

        private protected AlgebraicTerm(Term[] terms, Term higherOrder)
        {
            this.Terms = terms;
            this.HigherOrder = higherOrder;
        }

        public override Term HigherOrder { get; }

        protected abstract Term OnCreate(Term[] terms, Term higherOrder);

        public override sealed Term Infer(InferContext context)
        {
            var terms = this.Terms.Select(term => term.Infer(context)).Memoize();
            var higherOrder = context.ResolveHigherOrder(this.HigherOrder);

            foreach (var term in terms)
            {
                context.Unify(term.HigherOrder, higherOrder);
            }

            return
                this.Terms.Zip(terms, (t0, t1) => t0.EqualsWithHigherOrder(t1)).All(r => r) &&
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    this.OnCreate(terms, higherOrder);
        }

        public override sealed Term Fixup(FixupContext context)
        {
            var terms = this.Terms.Select(term => term.Fixup(context)).Memoize();
            var higherOrder = this.HigherOrder.Fixup(context);

            return
                this.Terms.Zip(terms, (t0, t1) => t0.EqualsWithHigherOrder(t1)).All(r => r) &&
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    this.OnCreate(terms, higherOrder);
        }

        public override sealed Term Reduce(ReduceContext context)
        {
            var terms = this.Terms.Select(term => term.Reduce(context)).Memoize();
            var higherOrder = this.HigherOrder.Reduce(context);

            return
                this.Terms.Zip(terms, (t0, t1) => t0.EqualsWithHigherOrder(t1)).All(r => r) &&
                this.HigherOrder.EqualsWithHigherOrder(higherOrder) ?
                    this :
                    this.OnCreate(terms, higherOrder);
        }

        internal abstract IEnumerable<Term> InternalFlatten();

        public Term[] Flatten() =>
            this.InternalFlatten().Memoize();

        public void Deconstruct(out Term[] terms) =>
            terms = this.Flatten();

        public void Deconstruct(out Term[] terms, out Term higherOrder)
        {
            terms = this.Flatten();
            higherOrder = this.HigherOrder;
        }
    }

    public abstract class AlgebraicTerm<T> : AlgebraicTerm
        where T : AlgebraicTerm
    {
        protected AlgebraicTerm(Term[] terms, Term higherOrder) :
            base(terms, higherOrder)
        { }

        protected override bool OnEquals(EqualsContext context, Term? other) =>
            other is T term ?
                this.Terms.SequenceEqual(term.Terms) :
                false;

        internal override IEnumerable<Term> InternalFlatten() =>
            this.Terms.SelectMany(term =>
            term is T algebraic ?
                algebraic.InternalFlatten() :
                new[] { term });
    }
}
