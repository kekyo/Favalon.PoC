using Favalon.Expressions;
using System.Linq;

namespace Favalon.Terms
{
    public sealed class ApplyTerm : Term
    {
        public readonly Term Function;
        public readonly Term Argument;

        public ApplyTerm(Term function, Term argument)
        {
            this.Function = function;
            this.Argument = argument;
        }

        protected override Expression VisitInfer(IInferContext context) =>
            new ApplyExpression(
                this.Function.VisitInferCore(context),
                this.Argument.VisitInferCore(context));

        public override bool Equals(Term? rhs) =>
            rhs is ApplyTerm apply ?
                (this.Function.Equals(apply.Function) && this.Argument.Equals(apply.Argument)) :
            false;

        public override string ToString() =>
            $"{this.GetType().Name}: {this.Function} {this.Argument}";
    }
}
