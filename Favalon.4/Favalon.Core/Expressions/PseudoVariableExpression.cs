using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public abstract class PseudoVariableExpression : VariableExpression, IEquatable<PseudoVariableExpression?>
    {
        internal PseudoVariableExpression() :
            base(null!)
        { }

        public override int GetHashCode() =>
            0;

        public bool Equals(PseudoVariableExpression? other) =>
            (other == null) ? false :
            object.ReferenceEquals(this, other);

        public override bool Equals(VariableExpression? other) =>
            this.Equals(other as PseudoVariableExpression);

        public override bool Equals(object obj) =>
            this.Equals(obj as PseudoVariableExpression);
    }
}
