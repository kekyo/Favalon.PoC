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
        private sealed class Node
        {
            public readonly HashSet<Unification> Unifications;

            public Node() =>
                this.Unifications = new HashSet<Unification>();

            public override string ToString() =>
                "[" + StringUtilities.Join(",", this.Unifications.Select(unification => unification.ToString())) + "]";
        }
        
        private readonly Dictionary<IPlaceholderTerm, Node> topology =
            new Dictionary<IPlaceholderTerm, Node>(IdentityTermComparer.Instance);
        private readonly Dictionary<IPlaceholderTerm, IPlaceholderTerm> aliases =
            new Dictionary<IPlaceholderTerm, IPlaceholderTerm>(IdentityTermComparer.Instance);

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

        [DebuggerStepThrough]
        private bool InternalAdd(
            IPlaceholderTerm placeholder,
            IExpression expression,
            UnificationPolarities polarity)
        {
            var ph = this.aliases.TryGetValue(placeholder, out var pha) ?
                pha : placeholder;
            var ex = expression is IPlaceholderTerm exph ?
                this.aliases.TryGetValue(exph, out var exa) ? exa : exph :
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
                    switch
                        (this.aliases.TryGetValue(fph, out var faph) ? faph : null,
                         this.aliases.TryGetValue(tph, out var taph) ? taph : null)
                    {
                        case (IPlaceholderTerm _, null):
                            this.AddBoth(
                                faph,
                                to);
                            break;
                        case (null, IPlaceholderTerm _):
                            this.AddBoth(
                                from,
                                taph);
                            break;
                        case (null, null):
                            this.aliases.Add(fph, tph);
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
            if (this.topology.TryGetValue(placeholder, out var node))
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

            return placeholder;
        }
        
        public IExpression Resolve(ITypeCalculator calculator, IPlaceholderTerm placeholder)
        {
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
                case (IPlaceholderTerm _, _):
                    if (inMost0 is IPlaceholderTerm imph0)
                    {
                        var inMost1 = this.InternalResolve(
                            ResolveContext.Create(
                                calculator,
                                UnificationPolarities.In,
                                AndExpression.Create),
                            imph0);
                        return inMost1;
                    }
                    return inMost0;
                case (_, IPlaceholderTerm _):
                    if (outMost0 is IPlaceholderTerm omph0)
                    {
                        var outMost1 = this.InternalResolve(
                            ResolveContext.Create(
                                calculator,
                                UnificationPolarities.Out,
                                OrExpression.Create),
                            omph0);
                        return outMost1;
                    }
                    return outMost0;
                default:
                    if (inMost0 is IPlaceholderTerm imph1)
                    {
                        var inMost1 = this.InternalResolve(
                            ResolveContext.Create(
                                calculator,
                                UnificationPolarities.In,
                                AndExpression.Create),
                            imph1);
                        return inMost1;
                    }
                    return inMost0;
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
                        "    {0} [label=\"{1}\",shape=circle];",
                        ToSymbolString(entry.Key).symbol,
                        entry.Key.Symbol);
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
