using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using Favalet.Internal;

namespace Favalet.Contexts.Unifiers
{
    [DebuggerDisplay("{View}")]
    internal sealed class Topology
    {
        private sealed class Node
        {
            public readonly HashSet<Unification> Unifications;
            public bool IsScopeWall { get; private set; }

            public Node() =>
                this.Unifications = new HashSet<Unification>();

            public void SetScopeWall() =>
                this.IsScopeWall = true;

            public override string ToString() =>
                (this.IsScopeWall ? "* [" : "[") +
                StringUtilities.Join(",", this.Unifications.Select(unification => unification.ToString())) + "]";
        }
        
        private struct ResolveResult
        {
            public readonly IExpression Bottom;
            public readonly IExpression Result;

            private ResolveResult(IExpression bottom, IExpression result)
            {
                this.Bottom = bottom;
                this.Result = result;
            }

            public ResolveResult Update(IExpression result) =>
                new ResolveResult(this.Bottom, result);

            public override string ToString() =>
                object.ReferenceEquals(this.Bottom, this.Result) ?
                    this.Result.GetPrettyString(PrettyStringTypes.Minimum) :
                    $"{this.Result.GetPrettyString(PrettyStringTypes.Minimum)} [{this.Bottom.GetPrettyString(PrettyStringTypes.Minimum)}]";

            public static ResolveResult Create(IExpression result) =>
                new ResolveResult(result, result);
            public static ResolveResult Create(IExpression bottom, IExpression result) =>
                new ResolveResult(bottom, result);
        }

        private readonly Dictionary<IPlaceholderTerm, Node> topology =
            new Dictionary<IPlaceholderTerm, Node>(IdentityTermComparer.Instance);

#if DEBUG
        private IExpression targetRoot;
#endif

        [DebuggerStepThrough]
        private Topology(IExpression targetRoot)
        {
#if DEBUG
            this.targetRoot = targetRoot;
#endif            
        }

        [DebuggerStepThrough]
        private bool AddIf(
            IPlaceholderTerm placeholder,
            IExpression expression,
            UnificationPolarities polarity,
            bool isScopeWall)
        {
            if (!this.topology.TryGetValue(placeholder, out var node))
            {
                node = new Node();
                this.topology.Add(placeholder, node);
            }

            if (isScopeWall)
            {
                node.SetScopeWall();
            }

            var unification = Unification.Create(expression, polarity);
            return node.Unifications.Add(unification);
        }

        [DebuggerStepThrough]
        public void AddForward(
            IPlaceholderTerm placeholder,
            IExpression from,
            bool isScopeWall)
        {
            this.AddIf(placeholder, from, UnificationPolarities.Out, isScopeWall);
            if (from is IPlaceholderTerm ei)
            {
                this.AddIf(ei, placeholder, UnificationPolarities.In, isScopeWall);
            }
        }

        [DebuggerStepThrough]
        public void AddBackward(
            IPlaceholderTerm placeholder,
            IExpression to,
            bool isScopeWall)
        {
            this.AddIf(placeholder, to, UnificationPolarities.In, isScopeWall);
            if (to is IPlaceholderTerm ei)
            {
                this.AddIf(ei, placeholder, UnificationPolarities.Out, isScopeWall);
            }
        }

        private ResolveResult InternalResolve(
            ITypeCalculator calculator, 
            IPlaceholderTerm placeholder,
            UnificationPolarities targetPolarity,
            Func<IExpression, IExpression, IExpression> creator,
            Dictionary<IPlaceholderTerm, ResolveResult> cache)
        {
            if (this.topology.TryGetValue(placeholder, out var node))
            {
                var unifications = node.Unifications.
                    Where(unification => unification.Polarity == targetPolarity).
                    ToArray();
                if (unifications.Length >= 1)
                {
                    var results = unifications.Select(unification =>
                        {
                            if (unification.Expression is IPlaceholderTerm ph)
                            {
                                if (!cache.TryGetValue(ph, out var cached))
                                {
                                    var result = this.InternalResolve(
                                        calculator,
                                        ph,
                                        targetPolarity,
                                        creator,
                                        cache);
                                    
                                    if ((targetPolarity == UnificationPolarities.Out) &&
                                        node.IsScopeWall)
                                    {
                                        // Force places this placeholder if it's out polarity and a scope wall.
                                        return result.Update(placeholder);
                                    }
                                    else
                                    {
                                        return result;
                                    }
                                }
                                else
                                {
                                    return cached;
                                }
                            }
                            else
                            {
                                return ResolveResult.Create(unification.Expression);
                            }
                        }).
                        ToArray();
                    
                    var bottomCombined = LogicalCalculator.ConstructNested(
                        results.
                            Select(result => result.Bottom).
                            ToArray(),
                        creator)!;
                    var bottomCalculated = calculator.Compute(bottomCombined);

                    var resultCombined = LogicalCalculator.ConstructNested(
                        results.
                            Select(result => result.Result).
                            ToArray(),
                        creator)!;
                    var resultCalculated = calculator.Compute(resultCombined);

                    var resultFinal = ResolveResult.Create(bottomCalculated, resultCalculated);
                    cache.Add(placeholder, resultFinal);
                    return resultFinal;
                }
            }

            var result = ResolveResult.Create(placeholder);
            cache.Add(placeholder, result);
            return result;
        }

        public IExpression Resolve(ITypeCalculator calculator, IPlaceholderTerm placeholder)
        {
            bool ContainsPlaceholder(IExpression expression) =>
                expression switch
                {
                    IPlaceholderTerm _ => true,
                    IParentExpression parent => parent.Children.Any(ContainsPlaceholder),
                    _ => false
                };
            
            var forwardResult = this.InternalResolve(
                calculator,
                placeholder,
                UnificationPolarities.In,
                AndExpression.Create,
                new Dictionary<IPlaceholderTerm, ResolveResult>(IdentityTermComparer.Instance));
            if (!ContainsPlaceholder(forwardResult.Result))
            {
                return forwardResult.Result;
            }

            var backwardResult = this.InternalResolve(
                calculator,
                placeholder,
                UnificationPolarities.Out,
                OrExpression.Create,
                new Dictionary<IPlaceholderTerm, ResolveResult>(IdentityTermComparer.Instance));
            if (!ContainsPlaceholder(backwardResult.Result))
            {
                return backwardResult.Result;
            }
            
            // Higher recommend if it isn't a single placeholder
            if (!(forwardResult.Result is IPlaceholderTerm) &&
                !(forwardResult.Result is IParentExpression))
            {
                return forwardResult.Result;
            }
            if (!(forwardResult.Bottom is IPlaceholderTerm) &&
                !(forwardResult.Bottom is IParentExpression))
            {
                return forwardResult.Bottom;
            }

            if (!(backwardResult.Result is IPlaceholderTerm) &&
                !(backwardResult.Result is IParentExpression))
            {
                return backwardResult.Result;
            }
            if (!(backwardResult.Bottom is IPlaceholderTerm) &&
                !(backwardResult.Bottom is IParentExpression))
            {
                return backwardResult.Bottom;
            }

            if (forwardResult.Result is IPlaceholderTerm)
            {
                return forwardResult.Result;
            }
            else if (backwardResult.Result is IPlaceholderTerm)
            {
                return backwardResult.Result;
            }
            else
            {
                return forwardResult.Result;
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
                    if (this.topology.TryGetValue(targetPlaceholder, out var node))
                    {
                        if (node.Unifications is IPlaceholderTerm pnext)
                        {
                            targetPlaceholder = pnext;
                            continue;
                        }
                        else
                        {
                            Validate(marker, (IIdentityTerm)node.Unifications);
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
        public void SetTargetRoot(IExpression targetRoot) =>
            this.targetRoot = targetRoot;

        public string View
        {
            [DebuggerStepThrough]
            get => StringUtilities.Join(
                Environment.NewLine,
                this.topology.
                    OrderBy(entry => entry.Key, IdentityTermComparer.Instance).
                    SelectMany(entry =>
                        entry.Value.Unifications.Select(unification =>
                            string.Format(
                                "{0}{1} {2}",
                                entry.Key.Symbol,
                                entry.Value.IsScopeWall ? "*" : "",
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
#if DEBUG
                tw.WriteLine(
                    "    graph [label=\"{0}\"];",
                    this.targetRoot.GetPrettyString(PrettyStringTypes.ReadableAll));
                tw.WriteLine();
#endif
                foreach (var entry in this.topology.
                    OrderBy(entry => entry.Key, IdentityTermComparer.Instance))
                {
                    tw.WriteLine(
                        "    p{0} [label=\"{1}\",shape={2}];",
                        entry.Key.Index,
                        entry.Key.Symbol,
                        entry.Value.IsScopeWall ? "doublecircle" : "circle");
                }
                
                foreach (var label in this.topology.
                    SelectMany(entry => entry.Value.Unifications.
                        Where(unification => !(unification.Expression is IPlaceholderTerm)).
                        Select(unification => unification.Expression.GetPrettyString(PrettyStringTypes.Minimum))).
                    Distinct().
                    OrderBy(label => label))
                {
                    tw.WriteLine(
                        "    {0} [shape=box];",
                        label);
                }

                tw.WriteLine();
                    
                foreach (var entry in this.topology.
                    OrderBy(entry => entry.Key, IdentityTermComparer.Instance).
                    SelectMany(entry => entry.Value.Unifications.
                        Select(unification => unification.Polarity == UnificationPolarities.In ?
                            ($"p{entry.Key.Index}", unification.Expression) :
                            (unification.Expression is IPlaceholderTerm ph ?
                                $"p{ph.Index}" :
                                unification.Expression.GetPrettyString(PrettyStringTypes.Minimum),
                                entry.Key))).
                    Distinct())
                {
                    tw.WriteLine(
                        "    {0} -> {1};",
                        entry.Item1,
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
        public static Topology Create(IExpression targetRoot) =>
            new Topology(targetRoot);
    }
}
