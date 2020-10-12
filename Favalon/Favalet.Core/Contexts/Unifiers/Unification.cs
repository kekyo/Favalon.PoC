using System.Diagnostics;
using System.Runtime.InteropServices;
using Favalet.Expressions;

namespace Favalet.Contexts.Unifiers
{
    internal enum UnificationPolarities
    {
        // Indicate "==>", forward polarity, will make covariant (widen)
        Out,
        // Indicate "<==", backward polarity, will make contravariant (narrow)
        In,
        // Both direction "<=>"  
        Both
    }

    [DebuggerStepThrough]
    internal readonly struct Unification
    {
        public readonly IExpression Expression;
        public readonly UnificationPolarities Polarity;

        private Unification(
            IExpression expression,
            UnificationPolarities polarity)
        {
            this.Expression = expression;
            this.Polarity = polarity;
        }

        public bool Equals(Unification rhs) =>
            this.Expression.Equals(rhs.Expression) &&
            (this.Polarity == rhs.Polarity);

        public string ToString(PrettyStringTypes type)
        {
            var polarity = this.Polarity switch
            {
                UnificationPolarities.Out => "==>",
                UnificationPolarities.In => "<==",
                _ => "<=>"
            };
            return $"{polarity} {this.Expression.GetPrettyString(type)}";
        }
        public override string ToString() =>
            this.ToString(PrettyStringTypes.Readable);

        public static Unification Create(
            IExpression expression,
            UnificationPolarities polarity) =>
            new Unification(expression, polarity);
    }
}
