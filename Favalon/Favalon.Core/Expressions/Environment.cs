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
        private readonly Environment? parent;
        private readonly PlaceholderController placeholderController;
        private Dictionary<VariableExpression, TermExpression>? boundExpressions;

        private Environment(Environment parent)
        {
            this.parent = parent;
            this.placeholderController = parent.placeholderController;
        }

        private Environment() =>
            placeholderController = new PlaceholderController();

        public Environment NewScope() =>
            new Environment(this);

        internal PlaceholderExpression CreatePlaceholder(TermExpression higherOrder) =>
            placeholderController.Create(higherOrder);

        internal TermExpression? GetRelatedExpression(PlaceholderExpression placeholder) =>
            placeholderController.GetRelated(placeholder);

        internal TermExpression? GetBoundExpression(VariableExpression variable) =>
            this.Traverse(environment => environment.parent!).
            Select(environment => (environment.boundExpressions != null) ?
                environment.boundExpressions.TryGetValue(variable, out var expression) ? expression : null : null).
            FirstOrDefault(expression => expression != null);

        internal void SetBoundExpression(VariableExpression bound, TermExpression expression)
        {
            if (boundExpressions == null)
            {
                boundExpressions = new Dictionary<VariableExpression, TermExpression>();
            }
            boundExpressions[bound] = expression;
        }

        public void Bind(BoundVariableExpression bound, TermExpression expression) =>
            this.SetBoundExpression(bound, expression);

        public void Register(BoundVariableExpression variable) =>
            this.SetBoundExpression(variable, new FreeVariableExpression(variable.Name, variable.HigherOrder));

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
                        placeholderController.AddRelated(placeholder1, placeholder2);
                    }
                }
                else
                {
                    placeholderController.AddRelated(placeholder1, expression2);
                }
                return;
            }
            else if (expression2 is PlaceholderExpression placeholder2)
            {
                placeholderController.AddRelated(placeholder2, expression1);
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
                        this.SetBoundExpression(identity1, expression2);
                    }
                }
                else if (this.GetBoundExpression(identity1) is TermExpression resolved)
                {
                    this.Unify(expression2, resolved);
                }
                else
                {
                    // Unify expression2 into identity1.
                    this.SetBoundExpression(identity1, expression2);
                }
                return;
            }
            else if (expression2 is BoundVariableExpression identity2)
            {
                if (this.GetBoundExpression(identity2) is TermExpression resolved)
                {
                    this.Unify(expression1, resolved);
                }
                else
                {
                    // Unify expression1 into identity2.
                    this.SetBoundExpression(identity2, expression1);
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
            var inferred = Expression.VisitInferring(this, expression, context);
            var (_, resolved) = Expression.VisitResolving(this, inferred, context);
            placeholderController.Remove(context.TouchedPlaceholders);
            return resolved;
        }

        public static Environment Create() =>
            new Environment();
    }
}
