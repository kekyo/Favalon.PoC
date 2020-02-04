using Favalon.Terms.Algebraic;
using System.Collections.Generic;

namespace Favalon.Terms.Types
{
    public class TypeCalculator : AlgebraicCalculator
    {
        protected TypeCalculator()
        { }

        protected override Term Sum(IEnumerable<Term> terms) =>
            TermFactory.Sum(terms)!;

        public override Term? Widening(Term? to, Term? from)
        {
            switch (to, from)
            {
                // int->object: int->object <-- object->int
                case (LambdaTerm(Term toParameter, Term toBody), LambdaTerm(Term fromParameter, Term fromBody)):
                    var parameterTerm = this.Widening(fromParameter, toParameter) is Term ? toParameter : null;   // TODO: Narrowing
                    var bodyTerm = this.Widening(toBody, fromBody);
                    return parameterTerm is Term pt && bodyTerm is Term bt ?
                        LambdaTerm.From(pt, bt) :
                        null;

                // _[1]: _[1] <-- _[2]
                //case (PlaceholderTerm placeholder, PlaceholderTerm _):
                //    return placeholder;

                // _: _ <-- int
                // _: _ <-- (int + double)
                case (PlaceholderTerm placeholder, _):
                    return placeholder;

                default:
                    if (base.Widening(to, from) is Term result)
                    {
                        return result;
                    }
                    // null: int <-- _   [TODO: maybe?]
                    //else if (from is PlaceholderTerm placeholder)
                    //{
                    //    return null;
                    //}
                    else
                    {
                        return null;
                    }
            }
        }

        public static readonly new TypeCalculator Instance =
            new TypeCalculator();
    }
}
