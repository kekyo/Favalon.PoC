﻿using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class VariableExpression : IdentityExpression
    {
        internal VariableExpression(string name, Expression higherOrder) :
            base(higherOrder) =>
            this.Name = name;

        internal VariableExpression(string name) :
            base(UndefinedExpression.Instance) =>
            this.Name = name;

        public override string Name { get; }

        internal VariableExpression CreateWithPlaceholderIfUndefined(Environment environment, InferContext context)
        {
            if (this.HigherOrder is UndefinedExpression)
            {
                var placeholder = environment.CreatePlaceholder();
                var variable = new VariableExpression(this.Name, placeholder);
                environment.SetNamedExpression(this.Name, variable);

                return variable;
            }
            else
            {
                return new VariableExpression(this.Name, this.HigherOrder);
            }
        }

        protected internal override Expression Visit(Environment environment, InferContext context)
        {
            if (environment.TryGetNamedExpression(this.Name, out var resolved))
            {
                return new VariableExpression(this.Name, resolved.HigherOrder);
            }
            else
            {
                return this.CreateWithPlaceholderIfUndefined(environment, context);
            }
        }

        protected internal override TraverseResults Traverse(System.Func<Expression, int, Expression> yc, int rank) =>
            TraverseResults.RequeireHigherOrder;

        /////////////////////////////////////////////////////////////////////////

        public static implicit operator VariableExpression(string name) =>
            new VariableExpression(name);
    }
}
