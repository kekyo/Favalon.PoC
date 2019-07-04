using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Internals
{
    internal sealed class PlaceholderController
    {
        private int index = 1;

        public PlaceholderController()
        { }

        public PlaceholderExpression Create(Expression higherOrder) =>
            new PlaceholderExpression(index++, higherOrder);
    }
}
