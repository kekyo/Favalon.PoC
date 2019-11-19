using Favalon.Terms;
using Favalon.Tokens;
using System.Diagnostics;
using System.Globalization;

namespace Favalon.ParseRunners
{
    internal abstract class ParseRunner
    {
        protected ParseRunner()
        { }

        public abstract ParseRunnerResult Run(ParseRunnerContext context, Token token);

        public virtual ParseRunnerResult Finish(ParseRunnerContext context) =>
            ParseRunnerResult.Create(WaitingRunner.Instance, context.CurrentTerm);

        protected static Term CombineTerm(Term? left, Term? right)
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
                Debug.Assert(right != null);
                return right!;
            }
        }

        protected static ConstantTerm GetNumericConstant(string value, Signes preSign) =>
            new ConstantTerm(int.Parse(value, CultureInfo.InvariantCulture) * (int)preSign);
    }
}
