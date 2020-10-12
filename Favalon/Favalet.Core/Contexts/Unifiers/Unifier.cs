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
        IUnsafePlaceholderResolver,
        ITopology
    {
        private readonly Topology topology ;
        
        [DebuggerStepThrough]
        private Unifier(ITypeCalculator typeCalculator, IExpression targetRoot) :
            base(typeCalculator) =>
            this.topology = Topology.Create(targetRoot);

        [DebuggerStepThrough]
        public void SetTargetRoot(IExpression targetRoot) =>
            this.topology.SetTargetRoot(targetRoot);

        private bool InternalUnifyCore(
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
                    return true;
                case (IPlaceholderTerm fph, _, false, _):
                    this.topology.AddBackward(fph, to, false);
                    //this.topology.Validate(fp2);
                    return true;
                case (_, IPlaceholderTerm tph, true, _):
                     this.topology.AddBoth(tph, from, true);
                     //this.topology.Validate(tp2);
                     return true;
                case (IPlaceholderTerm fph, _, true, _):
                    this.topology.AddBoth(fph, to, true);
                    //this.topology.Validate(fp2);
                    return true;

                // Binary expression unification.
                case (IBinaryExpression fb, _, _, _):
                    this.InternalUnify(fb.Left, to, false, false);
                    this.InternalUnify(fb.Right, to, false, false);
                    return true;
                case (_, IBinaryExpression tb, _, _):
                    this.InternalUnify(from, tb.Left, false, false);
                    this.InternalUnify(from, tb.Right, false, false);
                    return true;

                // Applied function unification.
                case (IFunctionExpression(IExpression fp, IExpression fr),
                      IAppliedFunctionExpression(IExpression tp, IExpression tr),
                      _, _):
                    // unify(C +> A): But parameters aren't binder.
                    this.InternalUnify(tp, fp, false, true);
                    // unify(B +> D)
                    this.InternalUnify(fr, tr, false, true);
                    return true;

                // Function unification.
                case (IFunctionExpression(IExpression fp, IExpression fr),
                      IFunctionExpression(IExpression tp, IExpression tr),
                      _, _):
                    // unify(C +> A): Parameters are binder.
                    this.InternalUnify(tp, fp, true, true);
                    // unify(B +> D)
                    this.InternalUnify(fr, tr, false, true);
                    return true;
                
                case (_, _, _, true):
                    // Validate polarity.
                    // from <: to
                    var f = this.TypeCalculator.Compute(OrExpression.Create(from, to));
                    if (!this.TypeCalculator.Equals(f, to))
                    {
                        throw new ArgumentException(
                            $"Couldn't unify: {from.GetPrettyString(PrettyStringTypes.Minimum)} <: {to.GetPrettyString(PrettyStringTypes.Minimum)}");
                    }
                    return true;
            }

            return false;
        }

        private bool InternalUnify(
            IExpression from,
            IExpression to,
            bool isBound,
            bool raiseIfCouldNotUnify)
        {
            // Same as.
            if (this.TypeCalculator.ExactEquals(from, to))
            {
                return true;
            }

            switch (from, to)
            {
                // Ignore IIgnoreUnificationTerm unification.
                case (IIgnoreUnificationTerm _, _):
                case (_, IIgnoreUnificationTerm _):
                    return true;

                default:
                    // Unify higher order.
                    if (this.InternalUnify(
                        from.HigherOrder,
                        to.HigherOrder,
                        isBound,
                        raiseIfCouldNotUnify))
                    {
                        // Unify if succeeded higher order.
                        return this.InternalUnifyCore(
                            from,
                            to,
                            isBound,
                            raiseIfCouldNotUnify);
                    }
                    break;
            }

            return false;
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
