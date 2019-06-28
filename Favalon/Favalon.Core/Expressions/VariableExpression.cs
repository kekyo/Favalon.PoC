using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class VariableExpression : TermExpression
    {
        public readonly string Name;

        internal VariableExpression(string name, TermExpression higherOrder) :
            base(higherOrder) =>
            this.Name = name;

        public override string ReadableString =>
            this.Name;

        protected override Expression VisitInferring(Environment environment)
        {
            var higherOrder = VisitInferring(environment, this.HigherOrder);
            return new VariableExpression(this.Name, higherOrder);
        }
    }
}
