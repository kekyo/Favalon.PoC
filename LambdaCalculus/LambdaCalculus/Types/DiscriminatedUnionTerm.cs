using Favalon.AlgebricData;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Types
{
    public sealed class DiscriminatedUnionTerm : HigherOrderHoldTerm
    {
        public readonly Term[] Constructors;

        internal DiscriminatedUnionTerm(IEnumerable<Term> constructors, Term higherOrder) :
            base(higherOrder) =>
            this.Constructors = constructors.ToArray();

        public override Term Infer(InferContext context)
        {
            var constructors = this.Constructors.
                Select(constructor => constructor.Infer(context)).
                ToArray();
            var higherOrder = this.HigherOrder.Infer(context);

            if (object.ReferenceEquals(higherOrder, this.HigherOrder) &&
                constructors.Zip(this.Constructors, object.ReferenceEquals).All(r => r))
            {
                return this;
            }

            var term = new DiscriminatedUnionTerm(constructors, higherOrder);

            foreach (var constructor in constructors)
            {
                if (constructor is BindExpressionTerm bound)
                {
                    context.Unify(
                        bound.Body.HigherOrder is LambdaTerm lambda ?
                            lambda.Body :
                            bound.Body.HigherOrder,
                        term);
                }
            }

            return term;
        }

        public override Term Fixup(FixupContext context)
        {
            var constructors = this.Constructors.
                Select(constructor => constructor.Fixup(context)).
                ToArray();
            var higherOrder = this.HigherOrder.Fixup(context);

            if (object.ReferenceEquals(higherOrder, this.HigherOrder) &&
                constructors.Zip(this.Constructors, object.ReferenceEquals).All(r => r))
            {
                return this;
            }

            var term = new DiscriminatedUnionTerm(constructors, higherOrder);

            return term;
        }

        public override Term Reduce(ReduceContext context)
        {
            var term = new DiscriminatedUnionTerm(
                // Side effect: will bind constructor in BindExpressionTerm.
                this.Constructors.Select(constructor => constructor.Reduce(context)),
                this.HigherOrder.Reduce(context));

            return term;
        }

        public override bool Equals(Term? other) =>
            other is DiscriminatedUnionTerm rhs ? this.Constructors.SequenceEqual(rhs.Constructors) : false;
    }
}
