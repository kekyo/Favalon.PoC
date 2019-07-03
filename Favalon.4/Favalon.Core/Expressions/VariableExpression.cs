using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public abstract class VariableExpression : TermExpression, IEquatable<VariableExpression?>
    {
        private protected VariableExpression(TermExpression higherOrder) :
            base(higherOrder)
        { }

        public abstract string Name { get; }

        public override int GetHashCode() =>
            this.Name.GetHashCode();

        public virtual bool Equals(VariableExpression? other) =>
            (other == null) ? false :
            object.ReferenceEquals(this, other) ? true :
            this.Name.Equals(other.Name);

        public override bool Equals(object obj) =>
            this.Equals(obj as VariableExpression);
    }
}
