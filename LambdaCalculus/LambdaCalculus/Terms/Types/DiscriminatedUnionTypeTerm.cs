using Favalon.Terms.AlgebricData;
using System.Collections.Generic;
using System.Linq;

namespace Favalon.Terms.Types
{
    public sealed class DiscriminatedUnionTypeTerm : Term
    {
        private Term higherOrder;

        internal DiscriminatedUnionTypeTerm(Term[] constructors, Term higherOrder)
        {
            this.Constructors = constructors;
            this.higherOrder = higherOrder;
        }

        public override Term HigherOrder =>
            higherOrder;

        public Term[] Constructors { get; private set; }

        public override Term Infer(InferContext context)
        {
            var constructors = this.Constructors.
                Select(constructor => constructor.Infer(context)).
                ToArray();
            var higherOrder = this.higherOrder.Infer(context);

            if (object.ReferenceEquals(higherOrder, this.higherOrder) &&
                constructors.Zip(this.Constructors, object.ReferenceEquals).All(r => r))
            {
                return this;
            }

            var term = new DiscriminatedUnionTypeTerm(constructors, higherOrder);

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
            var higherOrder = this.higherOrder.Fixup(context);

            if (object.ReferenceEquals(higherOrder, this.higherOrder) &&
                constructors.Zip(this.Constructors, object.ReferenceEquals).All(r => r))
            {
                return this;
            }

            this.Constructors = constructors;
            this.higherOrder = higherOrder;

            return this;
        }

        public override Term Reduce(ReduceContext context)
        {
            var constructors = this.Constructors.
                // Cause side effect: will bind constructor in BindExpressionTerm.
                Select(constructor => constructor.Reduce(context)).
                ToArray();
            var higherOrder = this.higherOrder.Reduce(context);

            if (object.ReferenceEquals(higherOrder, this.higherOrder) &&
                constructors.Zip(this.Constructors, object.ReferenceEquals).All(r => r))
            {
                return this;
            }

            this.Constructors = constructors;
            this.higherOrder = higherOrder;

            return this;
        }

        public override bool Equals(Term? other) =>
            other is DiscriminatedUnionTypeTerm rhs ? this.Constructors.SequenceEqual(rhs.Constructors) : false;
    }
}
