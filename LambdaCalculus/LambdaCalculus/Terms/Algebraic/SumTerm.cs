using Favalon.Terms.Contexts;
using System.Linq;

namespace Favalon.Terms.Algebraic
{
    public class SumTerm : AlgebraicTerm<SumTerm>
    {
        protected SumTerm(Term[] terms, Term higherOrder) :
            base(terms, higherOrder)
        { }

        protected override Term OnCreate(Term[] terms, Term higherOrder) =>
            new SumTerm(terms, higherOrder);

        protected override string OnPrettyPrint(PrettyPrintContext context) =>
            Utilities.Join(" + ", this.Terms.Select(term => term.PrettyPrint(context)));

        public static SumTerm Create(Term[] terms, Term higherOrder) =>
            new SumTerm(terms, higherOrder);

        public static Term From(Term[] terms, Term higherOrder) =>
            terms.Length switch
            {
                0 => EmptyTerm.Create(higherOrder),
                1 => terms[0],
                _ => new SumTerm(terms, higherOrder)
            };
    }
}
