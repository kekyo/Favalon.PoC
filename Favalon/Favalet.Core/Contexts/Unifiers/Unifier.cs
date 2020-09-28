using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using Favalet.Expressions.Specialized;
using System;
using System.Diagnostics;

namespace Favalet.Contexts.Unifiers
{
    [DebuggerDisplay("{View}")]
    internal sealed class Unifier :
        FixupContext,  // Because used by "Simple" property implementation.
        IUnsafePlaceholderResolver
    {
        private readonly Topology topology ;
        
        [DebuggerStepThrough]
        private Unifier(ITypeCalculator typeCalculator, IExpression targetRoot) :
            base(typeCalculator) =>
            this.topology = Topology.Create(targetRoot);

        private void InternalUnifyCore(
            IExpression from,
            IExpression to,
            bool isBound,
            bool raiseIfCouldNotUnify)
        {
            Debug.Assert(!(from is IIgnoreUnificationTerm));
            Debug.Assert(!(to is IIgnoreUnificationTerm));

            switch (from, to, isBound, raiseIfCouldNotUnify)
            {
                // Placeholder unification.
                case (_, IPlaceholderTerm tph, false, _):
                    this.topology.AddForward(tph, from, false);
                    //this.topology.Validate(tp2);
                    break;
                case (IPlaceholderTerm fph, _, false, _):
                    this.topology.AddBackward(fph, to, false);
                    //this.topology.Validate(fp2);
                    break;
                case (_, IPlaceholderTerm tph, true, _):
                     this.topology.Add(tph, from, true);
                     //this.topology.Validate(tp2);
                    break;
                case (IPlaceholderTerm fph, _, true, _):
                    this.topology.Add(fph, to, true);
                    //this.topology.Validate(fp2);
                    break;

                // Function unification.
                case (IFunctionExpression(IExpression fp, IExpression fr),
                      IFunctionExpression(IExpression tp, IExpression tr),
                      _, _):
                    // unify(C +> A)
                    this.InternalUnify(tp, fp, true, true);
                    // unify(B +> D)
                    this.InternalUnify(fr, tr, false, true);
                    break;

                // Binary expression unification.
                case (IBinaryExpression fb, _, _, _):
                    this.InternalUnify(fb.Left, to, false, false);
                    this.InternalUnify(fb.Right, to, false, false);
                    break;
                case (_, IBinaryExpression tb, _, _):
                    this.InternalUnify(from, tb.Left, false, false);
                    this.InternalUnify(from, tb.Right, false, false);
                    break;
                
                case (_, _, _, true):
                    // Validate polarity.
                    // from <: to
                    var f = this.TypeCalculator.Compute(OrExpression.Create(from, to));
                    if (!this.TypeCalculator.Equals(f, to))
                    {
                        throw new ArgumentException(
                            $"Couldn't unify: {from.GetPrettyString(PrettyStringTypes.Minimum)} <: {to.GetPrettyString(PrettyStringTypes.Minimum)}");
                    }
                    break;
            }
        }

        private void InternalUnify(
            IExpression from,
            IExpression to,
            bool isBound,
            bool raiseIfCouldNotUnify)
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
                    this.InternalUnify(
                        from.HigherOrder,
                        to.HigherOrder,
                        isBound,
                        raiseIfCouldNotUnify);

                    // Unify.
                    this.InternalUnifyCore(
                        from,
                        to,
                        isBound,
                        raiseIfCouldNotUnify);
                    break;
            }
        }

        [DebuggerStepThrough]
        public void Unify(
            IExpression from,
            IExpression to) =>
            this.InternalUnify(from, to, false, true);

        [DebuggerStepThrough]
        public override IExpression? Resolve(IPlaceholderTerm placeholder)
        {
#if DEBUG
            //this.topology.Validate(identity);
#endif
            return this.topology.Resolve(this.TypeCalculator, placeholder);
        }

        [DebuggerStepThrough]
        public void SetTargetRoot(IExpression targetRoot) =>
            this.topology.SetTargetRoot(targetRoot);

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
        public static Unifier Create(ITypeCalculator typeCalculator, IExpression targetRoot) =>
            new Unifier(typeCalculator, targetRoot);
    }
}
