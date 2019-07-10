using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    public abstract class VariableExpression : Expression
    {
        protected VariableExpression(Expression higherOrder) :
            base(higherOrder)
        { }
    }
}
