using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public abstract class VariableExpression : Expression
    {
        private protected VariableExpression(Expression higherOrder) :
            base(higherOrder)
        { }
    }
}
