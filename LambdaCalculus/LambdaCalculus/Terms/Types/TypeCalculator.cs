using Favalon.Terms.Algebraic;

namespace Favalon.Terms.Types
{
    public class TypeCalculator : AlgebraicCalculator
    {
        protected TypeCalculator()
        { }

        public override Term? Widen(Term? to, Term? from)
        {
            switch (to, from)
            {
                // int->object: int->object <-- object->int
                case (LambdaTerm(Term toParameter, Term toBody), LambdaTerm(Term fromParameter, Term fromBody)):
                    var parameterTerm = this.Widen(fromParameter, toParameter) is Term ? toParameter : null;   // TODO: Narrowing
                    var bodyTerm = this.Widen(toBody, fromBody);
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
                    if (base.Widen(to, from) is Term result)
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
