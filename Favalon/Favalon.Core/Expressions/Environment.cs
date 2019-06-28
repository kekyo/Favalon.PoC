using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Favalon.Expressions.Internals;

namespace Favalon.Expressions
{
    public sealed class Environment
    {
        private readonly Environment? parent;
        private Dictionary<string, TermExpression>? boundExpressions;

        private Environment(Environment parent) =>
            this.parent = parent;

        private Environment()
        { }

        public Environment NewScope() =>
            new Environment(this);

        internal (VariableExpression bound, TermExpression expression) InternalBind(VariableExpression bound, TermExpression expression)
        {
            var ve = Expression.VisitInferring(this, expression);
            var vb = Expression.VisitInferring(this, bound);

            this.Unify(vb.HigherOrder, ve.HigherOrder);
            //this.SetBound(vb, ve);

            return (vb, ve);
        }

        public void Bind(VariableExpression bound, TermExpression expression) =>
            this.InternalBind(bound, expression);

        internal TermExpression? GetBound(VariableExpression bound) =>
            this.Traverse(environment => environment.parent!).
            Select(environment => (environment.boundExpressions != null) ?
                environment.boundExpressions.TryGetValue(bound.Name, out var expression) ? expression : null : null).
            FirstOrDefault(expression => expression != null);

        private void SetBound(VariableExpression bound, TermExpression expression)
        {
            if (boundExpressions == null)
            {
                boundExpressions = new Dictionary<string, TermExpression>();
            }
            boundExpressions[bound.Name] = expression;
        }

        internal void Unify(TermExpression expression1, TermExpression expression2)
        {
            // NOTE: Argument direction expression1 --> expression2 is important.
            //   It contains processing forward direction, but excepts identity expressions.

            // TODO: Occurrence check (detect recursive declaration)

            if (expression1.Equals(expression2))
            {
                return;
            }

            if ((expression1 is UndefinedExpression) || (expression2 is UndefinedExpression))
            {
                return;
            }

            // Pair of variables / one of identity
            if (expression1 is VariableExpression identity1)
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
            else if (expression2 is VariableExpression identity2)
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

        public TExpression Infer<TExpression>(TExpression expression)
            where TExpression : Expression
        {
            var phase1 = Expression.VisitInferring(this, expression);
            return phase1;
        }

        public static Environment Create() =>
            new Environment();
    }
}
