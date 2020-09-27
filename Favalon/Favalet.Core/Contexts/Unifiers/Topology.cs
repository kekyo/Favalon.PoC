using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using Favalet.Internal;

namespace Favalet.Contexts.Unifiers
{
    [DebuggerDisplay("{View}")]
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
        private Unification[] Lookup(IPlaceholderTerm placeholder) =>
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
            if (this.topology.TryGetValue(placeholder, out var us))
            {
                var unifications = us.
                    Where(unification => unification.Polarity == polarity).
                    ToArray();
                if (unifications.Length >= 1)
                {
                    var combined = LogicalCalculator.ConstructNested(unifications.
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
                        creator)!;

                    var result = calculator.Compute(combined);
                    cache.Add(placeholder, result);
                    return result;
                }
            }

            cache.Add(placeholder, placeholder);
            return placeholder;
        }

        public IExpression Resolve(ITypeCalculator calculator, IPlaceholderTerm placeholder)
        {
            bool ContainsPlaceholder(IExpression expression) =>
                expression switch
                {
                    IPlaceholderTerm _ =>
                        true,
                    IParentExpression parent =>
                        parent.Children.Any(ContainsPlaceholder),
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
            
            // Higher recommend if it isn't a single placeholder
            if (!(forward is IPlaceholderTerm) &&
                !(forward is IParentExpression))
            {
                return forward;
            }

            // Higher recommend if it isn't a single placeholder
            if (!(backward is IPlaceholderTerm) &&
                !(backward is IParentExpression))
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
            else if (expression is IParentExpression parent)
            {
                foreach (var child in parent.Children)
                {
                    this.Validate(marker.Fork(), child);
                }
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

        public string View
        {
            [DebuggerStepThrough]
            get => StringUtilities.Join(
                Environment.NewLine,
                this.topology.
                    OrderBy(entry => entry.Key, IdentityTermComparer.Instance).
                    SelectMany(entry =>
                        entry.Value.Select(unification =>
                            string.Format(
                                "{0} {1}",
                                entry.Key.Symbol,
                                unification.ToString(PrettyStringTypes.Minimum)))));
        }

        public string Dot
        {
            [DebuggerStepThrough]
            get
            {
                var tw = new StringWriter();
                tw.WriteLine("digraph topology");
                tw.WriteLine("{");

                foreach (var identity in this.topology.Keys.
                    OrderBy(identity => identity, IdentityTermComparer.Instance))
                {
                    tw.WriteLine(
                        "    p{0} [label=\"{1}\"]",
                        identity.Index,
                        identity.Symbol);
                }
                
                foreach (var label in this.topology.
                    SelectMany(entry => entry.Value.
                        Where(unification => !(unification.Expression is IPlaceholderTerm)).
                        Select(unification => unification.Expression.GetPrettyString(PrettyStringTypes.Minimum))).
                    Distinct().
                    OrderBy(label => label))
                {
                    tw.WriteLine(
                        "    {0} [shape=box]",
                        label);
                }

                tw.WriteLine();
                    
                foreach (var entry in this.topology.
                    OrderBy(entry => entry.Key, IdentityTermComparer.Instance).
                    SelectMany(entry => entry.Value.
                        Where(unification => unification.Polarity == UnificationPolarities.In).
                        Select(unification => (entry.Key, unification.Expression))))
                {
                    tw.WriteLine(
                        "    p{0} -> {1}",
                        entry.Key.Index,
                        entry.Item2 is IPlaceholderTerm ph ?
                            $"p{ph.Index}" :
                            entry.Item2.GetPrettyString(PrettyStringTypes.Minimum));
                }
                
                tw.WriteLine("}");

                return tw.ToString();
            }
        }

        [DebuggerStepThrough]
        public override string ToString() =>
            "Topology: " + this.View;

        [DebuggerStepThrough]
        public static Topology Create() =>
            new Topology();
    }
}
