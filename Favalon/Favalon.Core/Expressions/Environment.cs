using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class Environment
    {
        private sealed class IndexHolder
        {
            private int index;

            [DebuggerNonUserCode]
            public int Next() =>
                index++;
        }

        private readonly Environment? parent;
        private readonly IndexHolder indexHolder;
        private Dictionary<VariableExpression, TermExpression>? boundExpressions;

        private Environment(Environment parent)
        {
            this.parent = parent;
            this.indexHolder = parent.indexHolder;
        }

        private Environment() =>
            indexHolder = new IndexHolder();

        public Environment NewScope() =>
            new Environment(this);

        internal PlaceholderExpression CreatePlaceholder(TermExpression higherOrder) =>
            new PlaceholderExpression(indexHolder.Next(), higherOrder);

        internal (BoundVariableExpression bound, TermExpression expression) InternalBind(BoundVariableExpression bound, TermExpression expression, InferContext context)
        {
            var ve = Expression.VisitInferring(this, expression, context);
            var vb = Expression.VisitInferring(this, bound, context);

            this.Unify(ve.HigherOrder, vb.HigherOrder);
            this.SetBound(vb, ve);

            return (vb, ve);
        }

        public void Bind(BoundVariableExpression bound, TermExpression expression)
        {
            var context = new InferContext();
            this.InternalBind(bound, expression, context);
        }

        public void Register(BoundVariableExpression variable)
        {
            var context = new InferContext();
            var higherOrder = Expression.VisitInferringHigherOrder(this, variable.HigherOrder, context);
            var freeVariable = new FreeVariableExpression(variable.Name, higherOrder);
            this.SetBound(variable, freeVariable);
        }

        internal TermExpression? GetBound(VariableExpression variable) =>
            this.Traverse(environment => environment.parent!).
            Select(environment => (environment.boundExpressions != null) ?
                environment.boundExpressions.TryGetValue(variable, out var expression) ? expression : null : null).
            FirstOrDefault(expression => expression != null);

        private void SetBound(VariableExpression bound, TermExpression expression)
        {
            if (boundExpressions == null)
            {
                boundExpressions = new Dictionary<VariableExpression, TermExpression>();
            }
            boundExpressions[bound] = expression;
        }

        internal void Unify(TermExpression expression1, TermExpression expression2)
        {
            // NOTE: Argument direction expression1 --> expression2 is important.
            //   It contains processing forward direction, but excepts identity expressions.

            // TODO: Occurrence check (detect infinite recursive declaration)

            if (expression1.Equals(expression2))
            {
                return;
            }

            if ((expression1 is UndefinedExpression) || (expression2 is UndefinedExpression))
            {
                return;
            }

            // Pair of placehoders / one of placeholder
            if (expression1 is PlaceholderExpression placeholder1)
            {
                if (expression2 is PlaceholderExpression placeholder2)
                {
                    // Unify placeholder2 into placeholder1 if aren't same.
                    if (!placeholder1.Equals(placeholder2))
                    {
                        this.SetBound(placeholder1, placeholder2);
                    }
                }
                else
                {
                    this.SetBound(placeholder1, expression2);
                }
                return;
            }
            else if (expression2 is PlaceholderExpression placeholder2)
            {
                this.SetBound(placeholder2, expression1);
                return;
            }

            // Pair of variables / one of identity
            if (expression1 is BoundVariableExpression identity1)
            {
                if (expression2 is VariableExpression identity2)
                {
                    // Unify identity2 into identity1 if aren't same.
                    if (!identity1.Equals(identity2))
                    {
                        this.SetBound(identity1, expression2);
                    }
                }
                else if (this.GetBound(identity1) is TermExpression resolved)
                {
                    this.Unify(expression2, resolved);
                }
                else
                {
                    // Unify expression2 into identity1.
                    this.SetBound(identity1, expression2);
                }
                return;
            }
            else if (expression2 is BoundVariableExpression identity2)
            {
                if (this.GetBound(identity2) is TermExpression resolved)
                {
                    this.Unify(expression1, resolved);
                }
                else
                {
                    // Unify expression1 into identity2.
                    this.SetBound(identity2, expression1);
                }
                return;
            }

            // Unify lambdas each parameters and expressions.
            if ((expression1 is LambdaExpression lambda1) &&
                (expression2 is LambdaExpression lambda2))
            {
                this.Unify(lambda1.Parameter, lambda2.Parameter);
                this.Unify(lambda1.Expression, lambda2.Expression);
                return;
            }

            // TODO: raise error?
        }

        public TermExpression Infer(VariableExpression variable) =>
            this.Infer<TermExpression>(variable);

        public TExpression Infer<TExpression>(TExpression expression)
            where TExpression : TermExpression
        {
            var context = new InferContext();
            var visited = Expression.VisitInferring(this, expression, context);
            context.Resolve();
            return visited;
        }

        public static Environment Create() =>
            new Environment();
    }
}
