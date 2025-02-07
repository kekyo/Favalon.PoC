﻿using Favalon.Expressions.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class KindExpression : PseudoVariableExpression
    {
        private KindExpression()
        { }

        public override string Name =>
            "*";

        protected override (string formatted, bool requiredParentheses) FormatReadableString(FormatContext context) =>
            ("*", false);

        public override string ToString() =>
            $"Kind: *";

        protected override Expression VisitInferring(IInferringEnvironment environment, InferContext context) =>
            this;

        protected override (bool isResolved, Expression resolved) VisitResolving(IResolvingEnvironment environment, InferContext context) =>
            (false, this);

        public static readonly KindExpression Instance = new KindExpression();
    }
}
