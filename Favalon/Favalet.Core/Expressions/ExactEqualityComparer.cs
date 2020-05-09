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

using Favalet.Expressions.Specialized;
using System.Collections.Generic;

namespace Favalet.Expressions
{
    public sealed class ExactEqualityComparer : IEqualityComparer<IExpression>
    {
        private ExactEqualityComparer()
        { }

        public bool Equals(IExpression x, IExpression y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }

            return (x, y) switch
            {
                (_, TerminationTerm _) => false,
                (TerminationTerm _, _) => false,
                (_, UnspecifiedTerm _) => false,
                (UnspecifiedTerm _, _) => false,
                _ => x.Equals(y) && this.Equals(x.HigherOrder, y.HigherOrder),
            };
        }

        public int GetHashCode(IExpression? obj) =>
            obj!.GetHashCode();

        public static readonly ExactEqualityComparer Instance =
            new ExactEqualityComparer();
    }
}
