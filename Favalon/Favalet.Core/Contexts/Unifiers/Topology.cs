using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using Favalet.Internal;

namespace Favalet.Contexts.Unifiers
{
    internal sealed class Topology
    {
        private readonly Dictionary<IPlaceholderTerm, HashSet<Unification>> topology =
            new Dictionary<IPlaceholderTerm, HashSet<Unification>>(IdentityTermComparer.Instance);

        [DebuggerStepThrough]
        private Topology()
        {
        }

        [DebuggerStepThrough]
        private bool AddIf(IPlaceholderTerm placeholder, IExpression expression, UnificationPolarities polarity)
        {
            if (!this.topology.TryGetValue(placeholder, out var unifications))
            {
                unifications = new HashSet<Unification>();
                this.topology.Add(placeholder, unifications);
            }

            var unification = Unification.Create(expression, polarity);
            return unifications.Add(unification);
        }

        [DebuggerStepThrough]
        public void AddForward(IPlaceholderTerm placeholder, IExpression expression)
        {
            this.AddIf(placeholder, expression, UnificationPolarities.Out);
            if (expression is IPlaceholderTerm ei)
            {
                this.AddIf(ei, placeholder, UnificationPolarities.In);
            }
        }

        [DebuggerStepThrough]
        public void AddBackward(IPlaceholderTerm placeholder, IExpression expression)
        {
            this.AddIf(placeholder, expression, UnificationPolarities.In);
            if (expression is IPlaceholderTerm ei)
            {
                this.AddIf(ei, placeholder, UnificationPolarities.Out);
            }
        }

        [DebuggerStepThrough]
        public Unification[] Lookup(IPlaceholderTerm placeholder) =>
            this.topology.TryGetValue(placeholder, out var unifications) ?
                unifications.ToArray() :
                ArrayEx.Empty<Unification>();
        
        private IExpression InternalResolve(
            ITypeCalculator calculator, 
            IPlaceholderTerm placeholder,
            UnificationPolarities polarity,
            Func<IExpression, IExpression, IExpression> creator,
            Dictionary<IPlaceholderTerm, IExpression> cache)
        {
            var unifications = this.Lookup(placeholder);
            if (unifications.Length == 0)
            {
                cache.Add(placeholder, placeholder);
                return placeholder;
            }
            
            var combined = LogicalCalculator.ConstructNested(
                unifications.
                Where(unification => unification.Polarity == polarity).
                Select(unification =>
                {
                    if (unification.Expression is IPlaceholderTerm ph)
                    {
                        if (!cache.TryGetValue(ph, out var cached))
                        {
                            return this.InternalResolve(calculator, ph, polarity, creator, cache);
                        }
                        else
                        {
                            return cached;
                        }
                    }
                    else
                    {
                        return unification.Expression;
                    }
                }).
                ToArray(),
                creator);

            if (combined != null)
            {
                var result = calculator.Compute(combined);
                cache.Add(placeholder, result);
                return result;
            }
            else
            {
                cache.Add(placeholder, placeholder);
                return placeholder;
            }
        }

        public IExpression Resolve(ITypeCalculator calculator, IPlaceholderTerm placeholder)
        {
            bool ContainsPlaceholder(IExpression expression) =>
                expression switch
                {
                    IPlaceholderTerm _ =>
                        true,
                    IBinaryExpression binary =>
                        ContainsPlaceholder(binary.Left) || ContainsPlaceholder(binary.Right),
                    IFunctionExpression function =>
                        ContainsPlaceholder(function.Parameter) || ContainsPlaceholder(function.Result),
                    IApplyExpression apply =>
                        ContainsPlaceholder(apply.Function) || ContainsPlaceholder(apply.Argument),
                    _ =>
                        false
                };
            
            var forward = this.InternalResolve(
                calculator,
                placeholder,
                UnificationPolarities.In,
                AndExpression.Create,
                new Dictionary<IPlaceholderTerm, IExpression>(IdentityTermComparer.Instance));
            if (!ContainsPlaceholder(forward))
            {
                return forward;
            }

            var backward = this.InternalResolve(
                calculator,
                placeholder,
                UnificationPolarities.Out,
                OrExpression.Create,
                new Dictionary<IPlaceholderTerm, IExpression>(IdentityTermComparer.Instance));
            if (!ContainsPlaceholder(backward))
            {
                return backward;
            }

            // Higher recommend if it's a single placeholder term excepts myself....?
            if (!placeholder.Equals(forward))
            {
                return forward;
            }
            else if (!placeholder.Equals(backward))
            {
                return backward;
            }
            if (forward is IPlaceholderTerm)
            {
                return forward;
            }
            else if (backward is IPlaceholderTerm)
            {
                return backward;
            }
            else
            {
                return forward;
            }
        }

        #region Validator
        [DebuggerStepThrough]
        private void Validate(PlaceholderMarker marker, IExpression expression)
        {
            if (expression is IPlaceholderTerm placeholder)
            {
                this.Validate(marker, placeholder);
            }
            else if (expression is IBinaryExpression(IExpression left, IExpression right))
            {
                this.Validate(marker.Fork(), left);
                this.Validate(marker.Fork(), right);
            }
            else if (expression is IFunctionExpression(IExpression parameter, IExpression result))
            {
                this.Validate(marker.Fork(), parameter);
                this.Validate(marker.Fork(), result);
            }
            else if (expression is IApplyExpression(IExpression function, IExpression argument))
            {
                this.Validate(marker.Fork(), function);
                this.Validate(marker.Fork(), argument);
            }
        }

        [DebuggerStepThrough]
        private void Validate(PlaceholderMarker marker, IPlaceholderTerm placeholder)
        {
            var targetPlaceholder = placeholder;
            while (true)
            {
                if (marker.Mark(targetPlaceholder))
                {
                    if (this.topology.TryGetValue(targetPlaceholder, out var unifications))
                    {
                        if (unifications is IPlaceholderTerm pnext)
                        {
                            targetPlaceholder = pnext;
                            continue;
                        }
                        else
                        {
                            Validate(marker, (IIdentityTerm)unifications);
                        }
                    }

                    return;
                }
#if DEBUG
                Debug.WriteLine(
                    "Detected circular variable reference: " + marker);
                throw new InvalidOperationException(
                    "Detected circular variable reference: " + marker);
#else
                throw new InvalidOperationException(
                    "Detected circular variable reference: " + symbol);
#endif
            }
        }
        
        [DebuggerStepThrough]
        public void Validate(IPlaceholderTerm placeholder) =>
            this.Validate(PlaceholderMarker.Create(), placeholder);
        #endregion

        [DebuggerStepThrough]
        public override string ToString() =>
            StringUtilities.Join(
                Environment.NewLine,
                this.topology.
                    OrderBy(entry => entry.Key, IdentityTermComparer.Instance).
                    SelectMany(entry =>
                        entry.Value.Select(unification =>
                            string.Format(
                            "{0} {1}",
                            entry.Key.Symbol,
                            unification.ToString(PrettyStringTypes.Minimum)))));
        
        [DebuggerStepThrough]
        public static Topology Create() =>
            new Topology();
    }
}
