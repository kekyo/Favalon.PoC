using Favalon.Terms;
using Favalon.Tokens;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Favalon.ParseRunners
{
    internal abstract class ParseRunner
    {
        protected ParseRunner()
        { }

        public abstract ParseRunnerResult Run(ParseRunnerContext context, Token token);

        public virtual ParseRunnerResult Finish(ParseRunnerContext context) =>
            ParseRunnerResult.Create(WaitingRunner.Instance, context.CurrentTerm);

        protected static Term CombineTerms(Term? left, Term? right)
        {
            if (left != null)
            {
                if (right != null)
                {
                    return new ApplyTerm(left, right);
                }
                else
                {
                    return left;
                }
            }
            else
            {
                return right!;
            }
        }

        protected static Term CombineTerms(params Term?[] terms) =>
            terms.Aggregate(CombineTerms)!;

        protected static ConstantTerm GetNumericConstant(string value, Signes preSign) =>
            new ConstantTerm(int.Parse(value, CultureInfo.InvariantCulture) * (int)preSign);
    }
}
