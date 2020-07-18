using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalet.Expressions.Algebraic
{
    public interface IOrBinaryExpression : IBinaryExpression
    {
    }

    public sealed class OrBinaryExpression :
        BinaryExpression<IOrBinaryExpression>,
        IOrBinaryExpression
    {
        private OrBinaryExpression(IExpression left, IExpression right) :
            base(left, right)
        { }

        public override IExpression Reduce(IReduceContext context)
        {
            var left = this.Left.Reduce(context);
            var right = this.Right.Reduce(context);

            if (object.ReferenceEquals(this.Left, left) &&
                object.ReferenceEquals(this.Right, right))
            {
                return this;
            }
            else
            {
                return new OrBinaryExpression(left, right);
            }
        }

        public override string GetPrettyString(PrettyStringTypes type) =>
            type switch
            {
                PrettyStringTypes.Simple =>
                    $"({this.Left.GetPrettyString(type)} || {this.Right.GetPrettyString(type)})",
                _ =>
                    $"(OrBinary {this.Left.GetPrettyString(type)} {this.Right.GetPrettyString(type)})"
            };

        public static OrBinaryExpression Create(IExpression left, IExpression right) =>
            new OrBinaryExpression(left, right);
    }
}
