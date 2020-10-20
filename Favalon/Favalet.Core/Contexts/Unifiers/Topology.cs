using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using Favalet.Internal;

namespace Favalet.Contexts.Unifiers
{
    public interface ITopology
    {
        string View { get; }    
        string Dot { get; }    
    }
    
    [DebuggerDisplay("{View}")]
    internal sealed class Topology :
        ITopology
    {
        [DebuggerStepThrough]
        private sealed class Node
        {
            public readonly HashSet<Unification> Unifications;

            public Node() =>
                this.Unifications = new HashSet<Unification>();

            public void Merge(Node node)
            {
                foreach (var unification in node.Unifications)
                {
                    this.Unifications.Add(unification);
                }
            }

            public override string ToString() =>
                "[" + StringUtilities.Join(",", this.Unifications.Select(unification => unification.ToString())) + "]";
        }
        
        private readonly Dictionary<IPlaceholderTerm, Node> topology =
            new Dictionary<IPlaceholderTerm, Node>(IdentityTermComparer.Instance);
        private readonly Dictionary<IPlaceholderTerm, IPlaceholderTerm> aliases =
            new Dictionary<IPlaceholderTerm, IPlaceholderTerm>(IdentityTermComparer.Instance);

#if DEBUG
        private IExpression targetRoot;
#else
        private string targetRootString;
#endif

        [DebuggerStepThrough]
        private Topology(IExpression targetRoot)
        {
#if DEBUG
            this.targetRoot = targetRoot;
#else
            this.targetRootString =
                targetRoot.GetPrettyString(PrettyStringTypes.ReadableAll);
#endif
        }

        [DebuggerStepThrough]
        private bool InternalAddNormalized(
            IPlaceholderTerm placeholder,
            IExpression expression,
            UnificationPolarities polarity)
        {
            if (!this.topology.TryGetValue(placeholder, out var node))
            {
                node = new Node();
                this.topology.Add(placeholder, node);
            }

            var unification = Unification.Create(expression, polarity);
            return node.Unifications.Add(unification);
        }

        private IPlaceholderTerm? GetAlias(
            IPlaceholderTerm placeholder,
            IPlaceholderTerm? defaultValue) =>
            this.aliases.TryGetValue(placeholder, out var alias) ?
                this.GetAlias(alias, alias) : defaultValue;
        
        private bool InternalAdd(
            IPlaceholderTerm placeholder,
            IExpression expression,
            UnificationPolarities polarity)
        {
            var ph =
                this.GetAlias(placeholder, placeholder)!;
            var ex = expression is IPlaceholderTerm exph ?
                this.GetAlias(exph, exph)! :
                expression;

            return this.InternalAddNormalized(ph, ex, polarity);
        }

        //[DebuggerStepThrough]
        public void AddBoth(
            IExpression from,
            IExpression to)
        {
            switch (from, to)
            {
                case (IPlaceholderTerm fph, IPlaceholderTerm tph)
                    when !fph.Equals(tph):
                    switch (this.GetAlias(fph, null), this.GetAlias(tph, null))
                    {
                        case (IPlaceholderTerm faph, null):
                            if (!faph.Equals(to))
                            {
                                this.AddBoth(
                                    faph,
                                    to);
                            }
                            break;
                        case (null, IPlaceholderTerm taph):
                            if (!from.Equals(taph))
                            {
                                this.AddBoth(
                                    from,
                                    taph);
                            }
                            break;
                        case (null, null):
                            var formalDirection = IdentityTermComparer.Instance.Compare(fph, tph);
                            if (formalDirection > 0)
                            {
                                this.aliases.Add(fph, tph);
                            }
                            else if (formalDirection < 0)
                            {
                                this.aliases.Add(tph, fph);
                            }
                            break;
                    }
                    break;
                
                case (IPlaceholderTerm fph, _):
                    this.InternalAdd(
                        fph,
                        to,
                        UnificationPolarities.Both);
                    break;
                
                case (_, IPlaceholderTerm tph):
                    this.InternalAdd(
                        tph,
                        from,
                        UnificationPolarities.Both);
                    break;
            }
        }

        [DebuggerStepThrough]
        public void AddForward(
            IPlaceholderTerm placeholder,
            IExpression from)
        {
            this.InternalAdd(
                placeholder,
                from,
                UnificationPolarities.In);
            
            if (from is IPlaceholderTerm ei)
            {
                this.InternalAdd(
                    ei,
                    placeholder,
                    UnificationPolarities.Out);
            }
        }

        [DebuggerStepThrough]
        public void AddBackward(
            IPlaceholderTerm placeholder,
            IExpression to)
        {
            this.InternalAdd(
                placeholder,
                to,
                UnificationPolarities.Out);
            
            if (to is IPlaceholderTerm ei)
            {
                this.InternalAdd(
                    ei,
                    placeholder,
                    UnificationPolarities.In);
            }
        }

        public void NormalizeAliases()
        {
            // Will make aliases normalized topology excepts outside PlaceholderTerm instances.
            
            foreach (var entry in this.aliases)
            {
                if (this.topology.TryGetValue(entry.Key, out var source))
                {
                    if (this.topology.TryGetValue(entry.Value, out var destination))
                    {
                        destination.Merge(source);
                    }
                    else
                    {
                        this.topology.Add(entry.Value, source);
                    }
                    this.topology.Remove(entry.Key);
                }
            }

            foreach (var node in this.topology.Values)
            {
                foreach (var unification in node.Unifications)
                {
                    if (unification.Expression is IPlaceholderTerm placeholder &&
                        this.aliases.TryGetValue(placeholder, out var target))
                    {
                        unification.UpdateExpression(target);
                    }
                }
            }
        }

        #region Resolve
        [DebuggerStepThrough]
        private sealed class ResolveContext
        {
            private readonly ITypeCalculator calculator;
            private readonly Func<IExpression, IExpression, IExpression> creator;

            public readonly UnificationPolarities Polarity;

            private ResolveContext(
                ITypeCalculator calculator,
                UnificationPolarities polarity, 
                Func<IExpression, IExpression, IExpression> creator)
            {
                Debug.Assert(polarity != UnificationPolarities.Both);
                
                this.calculator = calculator;
                this.Polarity = polarity;
                this.creator = creator;
            }

            public IExpression? Compute(IExpression[] expressions) =>
                LogicalCalculator.ConstructNested(expressions, this.creator) is IExpression combined ?
                    this.calculator.Compute(combined) : null;
            
            public static ResolveContext Create(
                ITypeCalculator calculator,
                UnificationPolarities polarity, 
                Func<IExpression, IExpression, IExpression> creator) =>
                new ResolveContext(calculator, polarity, creator);
        }
        
        private IExpression InternalResolve(
            ResolveContext context,
            IPlaceholderTerm placeholder)
        {
            var ph = this.GetAlias(placeholder, placeholder)!;
            
            if (this.topology.TryGetValue(ph, out var node))
            {
                IExpression ResolveRecursive(
                    IExpression expression)
                {
                    switch (expression)
                    {
                        case IPlaceholderTerm ph:
                            return this.InternalResolve(context, ph);
                        case IParentExpression parent:
                            return parent.Create(
                                parent.Children.Select(child => ResolveRecursive(child)))!;
                        default:
                            return expression;
                    }
                }
            
                var expressions = node.Unifications.
                    Where(unification =>
                        (unification.Polarity == context.Polarity) ||
                        (unification.Polarity == UnificationPolarities.Both)).
                    Select(unification => ResolveRecursive(unification.Expression)).
                    ToArray();
                if (expressions.Length >= 1)
                {
                    var calculated = context.Compute(expressions)!;
                    return calculated;
                }
            }

            return ph;
        }
        
        public IExpression Resolve(ITypeCalculator calculator, IPlaceholderTerm placeholder)
        {
            // TODO: cache
            
            var outMost0 = this.InternalResolve(
                ResolveContext.Create(
                    calculator,
                    UnificationPolarities.Out,
                    OrExpression.Create),
                placeholder);
            var inMost0 = this.InternalResolve(
                ResolveContext.Create(
                    calculator,
                    UnificationPolarities.In,
                    AndExpression.Create),
                placeholder);

            switch (outMost0, inMost0)
            {
                case (IPlaceholderTerm _, IPlaceholderTerm imph0):
                    // inmost (narrow) has higher priority.
                    var inMost1 = this.InternalResolve(
                        ResolveContext.Create(
                            calculator,
                            UnificationPolarities.In,
                            AndExpression.Create),
                        imph0);
                    return inMost1;
                case (IPlaceholderTerm _, _):
                    return inMost0;
                case (_, IPlaceholderTerm _):
                    return outMost0;
                default:
                    // Combine both expressions.
                    return calculator.Compute(
                        AndExpression.Create(outMost0, inMost0));
            }
        }
        #endregion
        
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
                    "Detected circular variable reference: " + marker);
#endif
            }
        }
        
        [DebuggerStepThrough]
        public void Validate(IPlaceholderTerm placeholder) =>
            this.Validate(PlaceholderMarker.Create(), placeholder);
        #endregion

        [DebuggerStepThrough]
        public void SetTargetRoot(IExpression targetRoot) =>
#if DEBUG
            this.targetRoot = targetRoot;
#else
            this.targetRootString =
                targetRoot.GetPrettyString(PrettyStringTypes.ReadableAll);
#endif

        public string View
        {
            [DebuggerStepThrough]
            get => StringUtilities.Join(
                Environment.NewLine,
                this.topology.
                    // TODO: alias
                    OrderBy(entry => entry.Key, IdentityTermComparer.Instance).
                    SelectMany(entry =>
                        entry.Value.Unifications.Select(unification =>
                            $"{entry.Key.Symbol} {unification.ToString(PrettyStringTypes.Minimum)}")));
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
#else
                tw.WriteLine(
                    "    graph [label=\"{0}\"];",
                    this.targetRootString);
#endif
                tw.WriteLine();
                tw.WriteLine("    # nodes");

                var symbolMap = new Dictionary<IExpression, string>();
                
                (string symbol, IExpression expression) ToSymbolString(IExpression expression)
                {
                    switch (expression)
                    {
                        case IPlaceholderTerm ph:
                            return ($"ph{ph.Index}", expression);
                        case IParentExpression parent:
                            if (!symbolMap!.TryGetValue(parent, out var symbol2))
                            {
                                var index = symbolMap.Count;
                                symbol2 = $"pe{index}";
                                symbolMap.Add(parent, symbol2);
                            }
                            return (symbol2, parent);
                        default:
                            if (!symbolMap!.TryGetValue(expression, out var symbol1))
                            {
                                var index = symbolMap.Count;
                                symbol1 = $"ex{index}";
                                symbolMap.Add(expression, symbol1);
                            }
                            return (symbol1, expression);
                    }
                }

                foreach (var entry in this.topology.
                    Select(entry => (ph: entry.Key, label: entry.Key.Symbol)).
                    Concat(this.aliases.Select(entry => (ph: entry.Key, label: entry.Key.Symbol))).
                    Concat(this.aliases.Select(entry => (ph: entry.Value, label: entry.Value.Symbol))).
                    Distinct().
                    OrderBy(entry => entry.ph, IdentityTermComparer.Instance))
                {
                    tw.WriteLine(
                        "    {0} [label=\"{1}\",shape=circle];",
                        ToSymbolString(entry.ph).symbol,
                        entry.label);
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
                            IParentExpression parent =>
                                $"xlabel=\"{parent.GetPrettyString(PrettyStringTypes.Minimum)}\",label=\"" +
                                StringUtilities.Join("|", parent.Children.Select((_, index) => $"<i{index}>[{index}]")) +
                                "\",shape=record",
                            _ =>
                                $"label=\"{entry.expression.GetPrettyString(PrettyStringTypes.Minimum)}\",shape=box",
                        });
                }

                tw.WriteLine();
                tw.WriteLine("    # topology");

                IEnumerable<(string from, string to, string attribute)> ToSymbols(IPlaceholderTerm placeholder, Unification unification)
                {
                    var phSymbol = ToSymbolString(placeholder).symbol;
                    switch (unification.Polarity, unification.Expression)
                    {
                        case (UnificationPolarities.Out, IParentExpression parent):
                            return parent.Children.Select((_, index) => (phSymbol, $"{ToSymbolString(parent).symbol}:i{index}", ""));
                        case (UnificationPolarities.In, IParentExpression parent):
                            return parent.Children.Select((_, index) => ($"{ToSymbolString(parent).symbol}:i{index}", phSymbol, ""));
                        case (UnificationPolarities.Both, IParentExpression parent):
                            return parent.Children.Select((_, index) => ($"{ToSymbolString(parent).symbol}:i{index}", phSymbol, " [dir=none]"));
                        case (UnificationPolarities.Out, _):
                            return new[] { (phSymbol, ToSymbolString(unification.Expression).symbol, "") };
                        case (UnificationPolarities.In, _):
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
                        entry.from,
                        entry.to,
                        entry.attribute);
                }
                
                tw.WriteLine();
                tw.WriteLine("    # aliases");

                foreach (var entry in this.aliases.
                    Select(entry => (from: ToSymbolString(entry.Key).symbol, to: ToSymbolString(this.GetAlias(entry.Key, entry.Key)!).symbol)).
                    Distinct().
                    OrderBy(entry => entry.from))
                {
                    tw.WriteLine(
                        "    {0} -> {1} [dir=none];",
                        entry.from,
                        entry.to);
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
