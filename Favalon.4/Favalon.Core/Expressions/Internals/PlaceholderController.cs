using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Favalon.Expressions.Internals
{
    internal sealed class PlaceholderController
    {
        private int index;
        private readonly Dictionary<PlaceholderExpression, TermExpression> relatedExpressions =
            new Dictionary<PlaceholderExpression, TermExpression>();

        [DebuggerStepThrough]
        public PlaceholderExpression Create(TermExpression higherOrder) =>
            new PlaceholderExpression(index++, higherOrder);

        [DebuggerStepThrough]
        public void AddRelated(PlaceholderExpression placeholder, TermExpression expression) =>
            relatedExpressions[placeholder] = expression;

        public TermExpression? GetRelated(InferContext context, PlaceholderExpression placeholder)
        {
            var current = placeholder;
            PlaceholderExpression? found = null;
            while (true)
            {
                if (relatedExpressions.TryGetValue(current, out var expression))
                {
                    context.TouchedInResolving(current);

                    if (expression is PlaceholderExpression pe)
                    {
                        found = pe;
                    }
                    else if (expression is LambdaExpression le)
                    {
                        var rp = (le.Parameter is PlaceholderExpression lpp) ? (this.GetRelated(context, lpp) ?? lpp) : le.Parameter;
                        var re = (le.Expression is PlaceholderExpression lpe) ? (this.GetRelated(context, lpe) ?? lpe) : le.Expression;
                        var rho = (le.HigherOrder is PlaceholderExpression lpho) ? (this.GetRelated(context, lpho) ?? lpho) : le.HigherOrder;
                        return new LambdaExpression(rp, re, rho);
                    }
                    else
                    {
                        return expression;
                    }
                }
                else
                {
                    return found;
                }

                current = found;
            }
        }

        public void Remove(IEnumerable<PlaceholderExpression> placeholders)
        {
            // TODO: We have to except implicitly depending between the placeholder and another placeholder.

            // A --> B   (X)
            // B --> C   (X)
            // D --> B   * missed B
            // Remove list: A, B

            foreach (var placeholder in placeholders)
            {
                relatedExpressions.Remove(placeholder);
            }
        }
    }
}
