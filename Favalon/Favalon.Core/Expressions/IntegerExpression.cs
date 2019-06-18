using System;

namespace Favalon.Expressions
{
    public sealed class IntegerExpression : Expression
    {
        public readonly int Value;

        internal IntegerExpression(int value) :
            base(Int32Type) =>
            this.Value = value;

        public override string ReadableString =>
            this.Value.ToString();

        internal override Expression Visit(ExpressionEnvironment environment) =>
            this;

        internal override void Resolve(ExpressionEnvironment environment)
        {
        }

        private static readonly TypeExpression Int32Type = new TypeExpression("System.Int32");
    }
}
