using System;
using System.Diagnostics;

namespace Favalet.Expressions
{
    public interface IConstantTerm : ITerm
    {
        object Value { get; }
    }

    public sealed class ConstantTerm :
        Expression, IConstantTerm
    {
        public readonly object Value;

        private ConstantTerm(object value) =>
            this.Value = value;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        object IConstantTerm.Value =>
            this.Value;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(IConstantTerm rhs) =>
            this.Value.Equals(rhs.Value);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is IConstantTerm rhs && this.Equals(rhs);

        public IExpression Reduce(IReduceContext context) =>
            this;

        public override string GetPrettyString(PrettyStringTypes type) =>
            this.Value switch
            {
                string value => $"\"{value}\"",
                _ => this.Value.ToString()
            };

        public static ConstantTerm Create(object value) =>
            new ConstantTerm(value);
    }
}
