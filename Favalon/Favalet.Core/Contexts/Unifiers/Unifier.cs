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
            bool isBound)
        {
            Debug.Assert(!(from is IIgnoreUnificationTerm));
            Debug.Assert(!(to is IIgnoreUnificationTerm));

            switch (from, to, isBound)
            {
                // Placeholder unification.
                case (_, IPlaceholderTerm tph, false):
                    this.topology.AddForward(tph, from, false);
                    //this.topology.Validate(tp2);
                    break;
                case (IPlaceholderTerm fph, _, false):
                    this.topology.AddBackward(fph, to, false);
                    //this.topology.Validate(fp2);
                    break;
                case (_, IPlaceholderTerm tph, true):
                     this.topology.Add(tph, from, true);
                     //this.topology.Validate(tp2);
                     break;
                case (IPlaceholderTerm fph, _, true):
                    this.topology.Add(fph, to, true);
                    //this.topology.Validate(fp2);
                    break;

                // Function unification.
                case (IFunctionExpression(IExpression fp, IExpression fr),
                      IFunctionExpression(IExpression tp, IExpression tr), _):
                    // unify(C +> A)
                    this.InternalUnify(tp, fp, true);
                    // unify(B +> D)
                    this.InternalUnify(fr, tr, false);
                    break;
                
                default:
                    // Validate polarity.
                    // from <: to
                    var f = this.TypeCalculator.Compute(OrExpression.Create(from, to));
                    if (!this.TypeCalculator.Equals(f, to))
                    {
                        throw new ArgumentException(
                            $"Couldn't unify: {f.GetPrettyString(PrettyStringTypes.Minimum)} <: {to.GetPrettyString(PrettyStringTypes.Minimum)}");
                    }
                    break;
            }
        }

        private void InternalUnify(
            IExpression from,
            IExpression to,
            bool isBound)
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
                    this.InternalUnify(from.HigherOrder, to.HigherOrder, isBound);

                    // Unify.
                    this.InternalUnifyCore(from, to, isBound);
                    break;
            }
        }

        [DebuggerStepThrough]
        public void Unify(
            IExpression from,
            IExpression to) =>
            this.InternalUnify(from, to, false);

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
