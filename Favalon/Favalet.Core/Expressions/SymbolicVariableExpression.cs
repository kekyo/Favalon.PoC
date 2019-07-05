using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public abstract class SymbolicVariableExpression :
        VariableExpression, IEquatable<SymbolicVariableExpression?>
    {
        private protected SymbolicVariableExpression(string name, Expression higherOrder) :
            base(higherOrder) =>
            this.Name = name;

        public readonly string Name;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            this.Name;

        private protected TExpression VisitInferringImplicitly<TExpression>(
            Environment environment, Func<string, Expression, TExpression> generator, Expression higherOrderHint)
            where TExpression : VariableExpression
        {
            if (Lookup(environment, this) is Expression bound)
            {
                var higherOrder = Unify(environment, higherOrderHint, this.HigherOrder, bound.HigherOrder);
                return generator(this.Name, higherOrder);
            }
            else
            {
                var higherOrder = Unify(environment, higherOrderHint, this.HigherOrder);
                var variable = generator(this.Name, higherOrder);
                Memoize(environment, this, variable);
                return variable;
            }
        }

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public bool Equals(SymbolicVariableExpression? other) =>
            other?.Name.Equals(this.Name) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as SymbolicVariableExpression);
    }
}
