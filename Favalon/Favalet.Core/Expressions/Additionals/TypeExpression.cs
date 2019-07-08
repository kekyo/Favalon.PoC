using Favalet.Expressions.Internals;
using System;
using System.Collections.Generic;

namespace Favalet.Expressions.Additionals
{
    public sealed class TypeExpression : FreeVariableExpression
    {
        internal TypeExpression(string typeName) :
            base(typeName, KindExpression.Instance)
        { }
    }
}
