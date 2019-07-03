using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public abstract class Expression
    {
        protected Expression(Expression higherOrder) =>
            this.HigherOrder = higherOrder;

        public readonly Expression HigherOrder;

        protected abstract string FormatReadableString();
        protected static string FormatReadableString(Expression expression) =>
            expression.FormatReadableString();

        public string ReadableString =>
            (this.HigherOrder is Expression higherOrder) ?
                $"({this.FormatReadableString()}):({higherOrder.FormatReadableString()})" :
                this.FormatReadableString();

        public override string ToString()
        {
            var name = this.GetType().Name.Replace("Expression", string.Empty);
            return $"{name}: {this.ReadableString}";
        }
    }
}
