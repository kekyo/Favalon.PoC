using Favalon.Terms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Favalon.Expressions
{
    public abstract class Expression
    {
        private protected Expression() =>
            this.HigherOrder = null!;

        protected Expression(Expression higherOrder) =>
            this.HigherOrder = higherOrder;

        public Expression HigherOrder { get; private set; }

        private protected void SetHigherOrder(Expression higherOrder) =>
            this.HigherOrder = higherOrder;
    }

    public sealed class UninterruptedExpression : Expression
    {
        public readonly Term Term;

        public UninterruptedExpression(Term term) =>
            this.Term = term;
 
        public override string ToString() =>
            $"Uninterrupted({this.Term})";
    }

    public abstract class LiteralExpression : Expression
    {
        private protected LiteralExpression()
        { }

        protected LiteralExpression(Expression higherOrder) :
            base(higherOrder)
        { }

        public abstract object RawValue { get; }

        public override string ToString() =>
            this.RawValue?.ToString() ?? "(null)";
    }

    public class LiteralExpression<TValue> : LiteralExpression
    {
        public readonly TValue Value;

        public LiteralExpression(TValue value)
            : base() =>
            this.Value = value;

        public override object RawValue =>
            this.Value!;
    }

    public sealed class BooleanExpression : LiteralExpression<bool>
    {
        public BooleanExpression(bool value) :
            base(value)
        { }
    }

    public sealed class StringExpression : LiteralExpression<string>
    {
        public StringExpression(string value) :
            base(value)
        { }

        public override string ToString() =>
            $"\"{this.Value}\"";
    }

    public sealed class VariableExpression : Expression
    {
        public readonly string SymbolName;

        public VariableExpression(string symbolName) =>
            this.SymbolName = symbolName;

        public override string ToString() =>
            this.SymbolName;
    }

    public sealed class ApplyExpression : Expression
    {
        public readonly Expression Function;
        public readonly Expression Argument;

        public ApplyExpression(Expression function, Expression argument)
        {
            this.Function = function;
            this.Argument = argument;
        }

        public override string ToString() =>
            $"{this.Function} {this.Argument}";
    }
}
