using Favalet.Contexts;
using Favalet.Expressions.Specialized;
using System.Diagnostics;

namespace Favalet.Expressions.Algebraic
{
    public interface IOrExpression : IBinaryExpression
    {
    }

    public sealed class OrExpression :
        BinaryExpression<IOrExpression>,
        IOrExpression
    {
        [DebuggerStepThrough]
        private OrExpression(
            IExpression left, IExpression right, IExpression higherOrder) :
            base(left, right, higherOrder)
        { }

        [DebuggerStepThrough]
        internal override IExpression OnCreate(
            IExpression left, IExpression right, IExpression higherOrder) =>
            new OrExpression(left, right, higherOrder);

        public override string GetPrettyString(PrettyStringContext context) =>
            context.FinalizePrettyString(
                this,
                context.IsSimple ?
                    $"{this.Left.GetPrettyString(context)} || {this.Right.GetPrettyString(context)}" :
                    $"Or {this.Left.GetPrettyString(context)} {this.Right.GetPrettyString(context)}");

        [DebuggerStepThrough]
        public static OrExpression Create(
            IExpression left, IExpression right, IExpression higherOrder) =>
            new OrExpression(left, right, higherOrder);
        [DebuggerStepThrough]
        public static OrExpression Create(
            IExpression left, IExpression right) =>
            new OrExpression(left, right, UnspecifiedTerm.Instance);
    }
}
