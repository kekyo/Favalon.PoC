﻿using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class UnitExpression : ConstantExpression
    {
        private UnitExpression() :
            base(new TypeExpression(typeof(Unit).FullName))
        { }

        internal override object GetValue() =>
            Unit.Value;

        protected override (string formatted, bool requiredParentheses) FormatReadableString(FormatContext context) =>
            (Unit.Value.ToString(), false);

        public static readonly UnitExpression Instance = new UnitExpression();
    }
}
