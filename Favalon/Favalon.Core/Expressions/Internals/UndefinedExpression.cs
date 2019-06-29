﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions.Internals
{
    public sealed class UndefinedExpression : TermExpression
    {
        private UndefinedExpression() :
            base(null!)
        { }

        public override string ReadableString =>
            "?";

        public override string ToString() =>
            $"Undefined: ?";

        protected override Expression VisitInferring(Environment environment, InferContext context) =>
            context.Rank switch
            {
                0 => (Expression)environment.CreatePlaceholder(environment.CreatePlaceholder(KindExpression.Instance)),
                1 => environment.CreatePlaceholder(KindExpression.Instance),
                2 => KindExpression.Instance,
                _ => null!
            };

        public static readonly UndefinedExpression Instance = new UndefinedExpression();
    }
}
