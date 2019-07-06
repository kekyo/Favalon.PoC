using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Favalet.Expressions.Additionals
{
    public sealed class LiteralExpression : ValueExpression, IEquatable<LiteralExpression?>
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

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint)
        {
            var typedHigherOrder = new FreeVariableExpression(this.GetTypeName(), KindExpression.Instance);
            var higherOrder = environment.Unify(typedHigherOrder, higherOrderHint);

            return new LiteralExpression(this.Value, higherOrder);
        }

        protected override Expression VisitResolving(IResolvingEnvironment environment) =>
            this;

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(LiteralExpression? other) =>
            other?.Value.Equals(this.Value) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as LiteralExpression);
    }
}
