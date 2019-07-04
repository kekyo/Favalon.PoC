using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class FreeVariableExpression :
        VariableExpression, IEquatable<FreeVariableExpression?>
    {
        public FreeVariableExpression(string name, Expression higherOrder) :
            base(higherOrder) =>
        this.Name = name;

        public readonly string Name;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            this.Name;

        protected override Expression VisitInferring(Environment environment, Expression higherOrderHint)
        {
            if (Lookup(environment, this) is Expression bound)
            {
                var higherOrder = Unify(environment, higherOrderHint, this.HigherOrder, bound.HigherOrder);
                return new FreeVariableExpression(this.Name, higherOrder);
            }
            else
            {
                var higherOrder = Unify(environment, higherOrderHint, this.HigherOrder);
                return new FreeVariableExpression(this.Name, higherOrder);
            }
        }

        protected override Expression VisitResolving(Environment environment)
        {
            var higherOrder = VisitResolving(environment, this.HigherOrder);
            return new FreeVariableExpression(this.Name, higherOrder);
        }

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public bool Equals(FreeVariableExpression? other) =>
            other?.Name.Equals(this.Name) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as FreeVariableExpression);
    }
}
