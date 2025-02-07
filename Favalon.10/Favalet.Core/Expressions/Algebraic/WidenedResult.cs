﻿////////////////////////////////////////////////////////////////////////////
//
// Favalon - An Interactive Shell Based on a Typed Lambda Calculus.
// Copyright (c) 2018-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Favalet.Expressions.Algebraic
{
    public struct WidenedResult
    {
        public readonly IExpression? Expression;
        public readonly bool IsUnexpected;

        private WidenedResult(IExpression? expression, bool isUnexpected)
        {
            Debug.Assert((expression == null) || !isUnexpected);

            this.Expression = expression;
            this.IsUnexpected = isUnexpected;
        }

        public override string ToString() =>
            (this.Expression, this.IsUnexpected) switch
            {
                (_, true) => "Unexpected",
                (null, _) => "Empty",
                _ => this.Expression!.ToString()
            };

        public static WidenedResult Success(IExpression expression) =>
            new WidenedResult(expression, false);
        public static readonly WidenedResult Empty =
            new WidenedResult(null, false);
        public static readonly WidenedResult Unexpected =
            new WidenedResult(null, true);

        public static WidenedCombinedResult Combine(IEnumerable<WidenedResult> results)
        {
            var rs = results.Memoize();
            if (rs.Any(r => r.IsUnexpected))
            {
                return new WidenedCombinedResult(null, true);
            }
            else
            {
                return new WidenedCombinedResult(rs.Select(r => r.Expression).Memoize(), false);
            }
        }
    }

    public struct WidenedCombinedResult
    {
        public readonly IExpression?[]? Expressions;
        public readonly bool IsUnexpected;

        public WidenedCombinedResult(IExpression?[]? expressions, bool isUnexpected)
        {
            Debug.Assert((expressions == null) || !isUnexpected);

            this.Expressions = expressions;
            this.IsUnexpected = isUnexpected;
        }

        public override string ToString() =>
            (this.Expressions, this.IsUnexpected) switch
            {
                (_, true) => "Unexpected",
                (null, _) => "Empty",
                _ => StringUtilities.Join(",", this.Expressions!.Select(ex => ex?.ToString() ?? "(null)"))
            };
    }
}
