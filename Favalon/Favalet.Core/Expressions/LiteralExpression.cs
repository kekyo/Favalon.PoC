using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public sealed class LiteralExpression : Expression, IEquatable<LiteralExpression?>
    {
        internal LiteralExpression(object value, Expression higherOrder) :
            base(higherOrder) =>
            this.Value = value;

        public readonly object Value;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (this.Value is string) ? $"\"{this.Value}\"" : this.Value.ToString();

        private string GetTypeName()
        {
#if NETSTANDARD1_0
            var type = this.Value.GetType().GetTypeInfo();
#else
            var type = this.Value.GetType();
#endif
            if (type.IsPrimitive)
            {
                return "Numeric";
            }
            else
            {
                return type.FullName;
            }
        }

        protected override Expression VisitInferring(Environment environment, Expression higherOrderHint) =>
            new LiteralExpression(this.Value, new FreeVariableExpression(this.GetTypeName(), KindExpression.Instance));

        protected override Expression VisitResolving(Environment environment) =>
            this;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(LiteralExpression? other) =>
            other?.Value.Equals(this.Value) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as LiteralExpression);
    }
}
