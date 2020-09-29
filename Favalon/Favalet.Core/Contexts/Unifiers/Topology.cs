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
        
        private sealed class ResolveResult
        {
            public readonly IExpression Bottom;
            public readonly IExpression Result;

            private ResolveResult(IExpression bottom, IExpression result)
            {
                Debug.Assert(bottom != null);
                Debug.Assert(result != null);
                
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
        public void Add(
            IExpression from,
            IExpression to,
            bool isScopeWall)
        {
            if (from is IPlaceholderTerm fph)
            {
                this.AddIf(
                    fph,
                    to,
                    UnificationPolarities.Both,
                    isScopeWall);
            }
            
            if (to is IPlaceholderTerm tph)
            {
                this.AddIf(
                    tph,
                    from,
                    UnificationPolarities.Both,
                    isScopeWall);
            }
        }

        [DebuggerStepThrough]
        public void AddForward(
            IPlaceholderTerm placeholder,
            IExpression from,
            bool isScopeWall)
        {
            this.AddIf(
                placeholder,
                from,
                UnificationPolarities.Out,
                isScopeWall);
            
            if (from is IPlaceholderTerm ei)
            {
                this.AddIf(
                    ei,
                    placeholder,
                    UnificationPolarities.In,
                    isScopeWall);
            }
        }

        [DebuggerStepThrough]
        public void AddBackward(
            IPlaceholderTerm placeholder,
            IExpression to,
            bool isScopeWall)
        {
            this.AddIf(
                placeholder,
                to,
                UnificationPolarities.In,
                isScopeWall);
            
            if (to is IPlaceholderTerm ei)
            {
                this.AddIf(
                    ei,
                    placeholder,
                    UnificationPolarities.Out,
                    isScopeWall);
            }
        }

        private ResolveResult InternalResolve(
            ITypeCalculator calculator, 
            IPlaceholderTerm placeholder,
            UnificationPolarities targetPolarity,
            Func<IExpression, IExpression, IExpression> creator,
            HashSet<IPlaceholderTerm> visited,
            Dictionary<IPlaceholderTerm, ResolveResult> cache)
        {
            var v = visited.Add(placeholder);
            Debug.Assert(v);

            if (this.topology.TryGetValue(placeholder, out var node))
            {
                var unifications = node.Unifications.
                    Where(unification =>
                        (unification.Polarity == targetPolarity) ||
                        (unification.Polarity == UnificationPolarities.Both)).
                    ToArray();
                if (unifications.Length >= 1)
                {
                    var results = unifications.
                        Collect(unification =>
                        {
                            if (!(unification.Expression is IPlaceholderTerm uph))
                            {
                                return ResolveResult.Create(unification.Expression);
                            }

                            if (cache.TryGetValue(uph, out var cached))
                            {
                                return cached;
                            }

                            if (visited.Contains(uph))
                            {
                                return null;
                            }

                            var ur = this.InternalResolve(
                                calculator,
                                uph,
                                targetPolarity,
                                creator,
                                visited,
                                cache);
                            
                            if ((targetPolarity != UnificationPolarities.In) &&
                                node.IsScopeWall)
                            {
                                // Force places this placeholder if it's out polarity and a scope wall.
                                return ur.Update(placeholder);
                            }
                            else
                            {
                                return ur;
                            }
                        }).
                        ToArray();

                    if (results.Length >= 1)
                    {
                        var bottomCombined = LogicalCalculator.ConstructNested(
                            results.Select(r => r.Bottom).ToArray(),
                            creator)!;
                        var bottomCalculated = calculator.Compute(bottomCombined);

                        var resultCombined = LogicalCalculator.ConstructNested(
                            results.Select(r => r.Result).ToArray(),
                            creator)!;
                        var resultCalculated = calculator.Compute(resultCombined);

                        var resultFinal = ResolveResult.Create(bottomCalculated, resultCalculated);
                        cache.Add(placeholder, resultFinal);
                        return resultFinal;
                    }
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
                new HashSet<IPlaceholderTerm>(IdentityTermComparer.Instance),
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
                new HashSet<IPlaceholderTerm>(IdentityTermComparer.Instance),
                new Dictionary<IPlaceholderTerm, ResolveResult>(IdentityTermComparer.Instance));
            if (!ContainsPlaceholder(backwardResult.Result))
            {
                return backwardResult.Result;
            }
            
            if (!ContainsPlaceholder(forwardResult.Bottom))
            {
                return forwardResult.Bottom;
            }
            if (!ContainsPlaceholder(backwardResult.Bottom))
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

                var parentSymbolMap = new Dictionary<IParentExpression, string>();
                
                (string symbol, IExpression expression) ToSymbolString(IExpression expression)
                {
                    switch (expression)
                    {
                        case IPlaceholderTerm ph:
                            return ($"p{ph.Index}", expression);
                        case IParentExpression parent:
                            if (!parentSymbolMap!.TryGetValue(parent, out var symbol))
                            {
                                var index = parentSymbolMap.Count;
                                symbol = $"c{index}";
                                parentSymbolMap.Add(parent, symbol);
                            }
                            return (symbol, parent);
                        default:
                            return (expression.GetPrettyString(PrettyStringTypes.Minimum), expression);
                    }
                }
                
                foreach (var entry in this.topology.
                    OrderBy(entry => entry.Key, IdentityTermComparer.Instance))
                {
                    tw.WriteLine(
                        "    {0} [label=\"{1}\",shape={2}];",
                        ToSymbolString(entry.Key).symbol,
                        entry.Key.Symbol,
                        entry.Value.IsScopeWall ? "doublecircle" : "circle");
                }

                foreach (var entry in this.topology.
                    SelectMany(entry =>
                        entry.Value.Unifications.
                        Where(unification => !(unification.Expression is IPlaceholderTerm)).
                        Select(unification => ToSymbolString(unification.Expression))).
                    Distinct().
                    OrderBy(entry => entry.symbol))
                {
                    tw.WriteLine(
                        "    {0} [{1}];",
                        entry.symbol,
                        entry.expression switch
                        {
                            IPlaceholderTerm _ => "shape=circle",
                            IParentExpression parent =>
                                $"xlabel=\"{parent.GetPrettyString(PrettyStringTypes.Minimum)}\",label=\"" +
                                StringUtilities.Join("|", parent.Children.Select((_, index) => $"<i{index}>[{index}]")) +
                                "\",shape=record",
                            _ => "shape=box"
                        });
                }

                tw.WriteLine();

                IEnumerable<(string, string, string)> ToSymbols(IPlaceholderTerm placeholder, Unification unification)
                {
                    var phSymbol = ToSymbolString(placeholder).symbol;
                    switch (unification.Polarity, unification.Expression)
                    {
                        case (UnificationPolarities.In, IParentExpression parent):
                            return parent.Children.Select((_, index) => (phSymbol, $"{ToSymbolString(parent).symbol}:i{index}", ""));
                        case (UnificationPolarities.Out, IParentExpression parent):
                            return parent.Children.Select((_, index) => ($"{ToSymbolString(parent).symbol}:i{index}", phSymbol, ""));
                        case (UnificationPolarities.Both, IParentExpression parent):
                            return parent.Children.Select((_, index) => ($"{ToSymbolString(parent).symbol}:i{index}", phSymbol, " [dir=none]"));
                        case (UnificationPolarities.In, _):
                            return new[] { (phSymbol, ToSymbolString(unification.Expression).symbol, "") };
                        case (UnificationPolarities.Out, _):
                            return new[] { (ToSymbolString(unification.Expression).symbol, phSymbol, "") };
                        case (UnificationPolarities.Both, _):
                            return new[] { (ToSymbolString(unification.Expression).symbol, phSymbol, " [dir=none]") };
                        default:
                            throw new InvalidOperationException();
                    }
                }
                    
                foreach (var entry in this.topology.
                    SelectMany(entry => entry.Value.Unifications.
                        SelectMany(unification => ToSymbols(entry.Key, unification))).
                    Distinct().
                    OrderBy(entry => entry.Item1))
                {
                    tw.WriteLine(
                        "    {0} -> {1}{2};",
                        entry.Item1,
                        entry.Item2,
                        entry.Item3);
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
