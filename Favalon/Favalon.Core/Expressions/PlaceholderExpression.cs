using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class PlaceholderExpression : Expression
    {
        public PlaceholderExpression(int index, Expression higherOrder) :
            base(higherOrder) =>
        this.Index = index;

        public readonly int Index;

        protected override string FormatReadableString() =>
            $"'{this.Index}";
    }
}
