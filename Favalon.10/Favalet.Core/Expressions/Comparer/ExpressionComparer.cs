////////////////////////////////////////////////////////////////////////////
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

using System;
using System.Collections.Generic;

namespace Favalet.Expressions.Comparer
{
    public sealed class ExpressionComparer : IComparer<IExpression>
    {
        private ExpressionComparer()
        { }

        int IComparer<IExpression>.Compare(IExpression x, IExpression y) =>
            Compare(x, y);

        public static int Compare(IExpression x, IExpression y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return 0;
            }

            switch (x, y)
            {
                case (IComparable<IExpression> cx, _): return cx.CompareTo(y);
                case (_, IComparable<IExpression> cy): return 0 - cy.CompareTo(x);
                default: return x.GetHashCode().CompareTo(y.GetHashCode());
            };
        }

        public static readonly IComparer<IExpression> Instance =
            new ExpressionComparer();
    }
}
