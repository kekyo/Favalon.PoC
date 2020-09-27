using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Contexts.Unifiers
{
    [DebuggerDisplay("{View}")]
    internal sealed class Unifier :
        FixupContext,  // Because used by "Simple" property implementation.
        IUnsafePlaceholderResolver
    {
        private readonly Topology topology = Topology.Create();
        
        [DebuggerStepThrough]
        private Unifier(ITypeCalculator typeCalculator) :
            base(typeCalculator)
        {
        }

        private void InternalUnify(
            IExpression from,
            IExpression to)
        {
            Debug.Assert(!(from is IIgnoreUnificationTerm));
            Debug.Assert(!(to is IIgnoreUnificationTerm));

            switch (from, to)
            {
                // Placeholder unification.
                case (_, IPlaceholderTerm tp2):
                    this.topology.AddForward(tp2, from);
                    //this.topology.Validate(tp2);
                    break;
                case (IPlaceholderTerm fp2, _):
                    this.topology.AddBackward(fp2, to);
                    //this.topology.Validate(fp2);
                    break;

                // Function unification.
                case (IFunctionExpression(IExpression fp, IExpression fr),
                      IFunctionExpression(IExpression tp, IExpression tr)):
                    // unify(C +> A)
                    this.Unify(tp, fp);
                    // unify(B +> D)
                    this.Unify(fr, tr);
                    break;
                
                default:
                    // Validate polarity.
                    // from <: to
                    var f = this.TypeCalculator.Compute(OrExpression.Create(from, to));
                    if (!this.TypeCalculator.Equals(f, to))
                    {
                        throw new ArgumentException("");
                    }
                    break;
            }
        }

        public void Unify(
            IExpression from,
            IExpression to)
        {
            // Same as.
            if (this.TypeCalculator.ExactEquals(from, to))
            {
                return;
            }

            switch (from, to)
            {
                // Ignore IIgnoreUnificationTerm unification.
                case (IIgnoreUnificationTerm _, _):
                case (_, IIgnoreUnificationTerm _):
                    break;

                default:
                    // Unify higher order.
                    this.Unify(from.HigherOrder, to.HigherOrder);

                    // Unify.
                    this.InternalUnify(from, to);
                    break;
            }
        }

        [DebuggerStepThrough]
        public override IExpression? Resolve(IPlaceholderTerm placeholder)
        {
#if DEBUG
            //this.topology.Validate(identity);
#endif
            return this.topology.Resolve(this.TypeCalculator, placeholder);
        }

        public string View
        {
            [DebuggerStepThrough]
            get => this.topology.View;
        }

        public string Dot
        {
            [DebuggerStepThrough]
            get => this.topology.Dot;
        }

        [DebuggerStepThrough]
        public override string ToString() =>
            "Unifier: " + this.View;
        
        [DebuggerStepThrough]
        public static Unifier Create(ITypeCalculator typeCalculator) =>
            new Unifier(typeCalculator);
    }
}
