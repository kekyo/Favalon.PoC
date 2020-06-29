using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet
{
    public interface IEquivalenceExpression : IExpression
    {
        IAndExpression Expression { get; }
    }

    public sealed class EquivalenceExpression :
        IEquivalenceExpression
    {
        private readonly IAndExpression Expression;

        private EquivalenceExpression(IAndExpression expression) =>
            this.Expression = expression;

        IAndExpression IEquivalenceExpression.Expression =>
            this.Expression;

        public override int GetHashCode() =>
            this.Expression.GetHashCode();

        public bool Equals(IEquivalenceExpression rhs) =>
            this.Expression.Equals(rhs.Expression);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            other is IEquivalenceExpression rhs && Equals(rhs);

        public IExpression Reduce(IReduceContext context)
        {
            var suppressed =
                this.Expression.Operands.
                Distinct().
                ToArray();

            return suppressed.Length switch
            {
                0 => AndExpression.Create(ArrayEx.Empty<IExpression>()),
                1 => suppressed[0],
                _ => Create(AndExpression.Create(suppressed))
            };
        }

        public static EquivalenceExpression Create(IAndExpression expression) =>
            new EquivalenceExpression(expression);
    }
}
