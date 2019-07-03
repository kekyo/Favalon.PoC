﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class VariableExpression : Expression
    {
        public VariableExpression(string name, Expression higherOrder) :
            base(higherOrder) =>
        this.Name = name;

        public readonly string Name;

        protected override string FormatReadableString() =>
            this.Name;
    }
}
