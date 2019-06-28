using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class UnitExpression : ConstantExpression
    {
        private UnitExpression() :
            base(new VariableExpression(typeof(Unit).FullName, KindExpression.Instance))
        { }

        internal override object GetValue() =>
            Unit.Value;

        public override string ReadableString =>
            Unit.Value.ToString();

        public static readonly UnitExpression Instance = new UnitExpression();
    }
}
