using System.Diagnostics;
using Favalet.Expressions;

namespace Favalet.Contexts.Unifiers
{
    internal enum UnificationPolarities
    {
        Covariance,
        Contravariance
    }
    
    [DebuggerStepThrough]
    internal readonly struct Unification
    {
        public readonly IExpression Expression;
        public readonly UnificationPolarities Polarity;

        private Unification(IExpression expression, UnificationPolarities polarity)
        {
            this.Expression = expression;
            this.Polarity = polarity;
        }

        public bool Equals(Unification rhs) =>
            this.Expression.Equals(rhs.Expression) &&
            (this.Polarity == rhs.Polarity);

        public string ToString(PrettyStringTypes type) =>
            this.Polarity switch
            {
                UnificationPolarities.Covariance => $"out,{this.Expression.GetPrettyString(type)}",
                UnificationPolarities.Contravariance => $"in,{this.Expression.GetPrettyString(type)}",
                _ => $"{this.Expression.GetPrettyString(type)}",
            };
        public override string ToString() =>
            this.ToString(PrettyStringTypes.Readable);

        public static Unification Create(IExpression expression, UnificationPolarities polarity) =>
            new Unification(expression, polarity);
    }
}