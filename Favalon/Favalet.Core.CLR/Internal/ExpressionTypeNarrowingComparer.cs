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

using Favalet.Expressions;
using Favalet.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Favalet.Internal
{
    internal sealed class ExpressionTypeNarrowingComparer : IComparer<IExpression>
    {
        private ExpressionTypeNarrowingComparer()
        { }

        private static int CompareTypes(Type tx, Type ty)
        {
            if (tx.Equals(ty))
            {
                return 0;
            }

            if (ty.IsConvertibleFrom(tx))
            {
                return 1;
            }
            if (tx.IsConvertibleFrom(ty))
            {
                return -1;
            }

            if (tx.IsInteger() && ty.IsInteger())
            {
                var txs = tx.SizeOf();
                var tys = ty.SizeOf();
                if (txs < tys)
                {
                    return -1;
                }
                if (txs > tys)
                {
                    return 1;
                }

                var txct = tx.IsClsCompliant();
                var tyct = ty.IsClsCompliant();
                if (txct && !tyct)
                {
                    return -1;
                }
                if (!txct && tyct)
                {
                    return 1;
                }
            }

            if (tx.IsInteger())
            {
                return -1;
            }
            if (ty.IsInteger())
            {
                return -1;
            }

            if (tx.IsPrimitive())
            {
                return -1;
            }
            if (ty.IsPrimitive())
            {
                return -1;
            }

            if (tx.IsValueType() && ty.IsValueType())
            {
                var txs = tx.SizeOf();
                var tys = ty.SizeOf();
                if (txs < tys)
                {
                    return -1;
                }
                if (txs > tys)
                {
                    return 1;
                }
            }

            if (tx.IsGenericType() && ty.IsGenericType())
            {
                if (!tx.IsGenericParameter && ty.IsGenericParameter)
                {
                    return -1;
                }
                if (tx.IsGenericParameter && !ty.IsGenericParameter)
                {
                    return 1;
                }
                if (!tx.IsGenericParameter && !ty.IsGenericParameter)
                {
                    var txgps = tx.GetGenericArguments();
                    var tygps = ty.GetGenericArguments();
                    if (txgps.Length < tygps.Length)
                    {
                        return -1;
                    }
                    if (txgps.Length > tygps.Length)
                    {
                        return 1;
                    }

                    if (txgps.
                        Zip(tygps, (x, y) => CompareTypes(x, y)).
                        FirstOrDefault(r => r != 0) is int r)
                    {
                        return r;
                    }
                }
            }

            // TODO: array

            // Not compatible types.
            return tx.FullName.CompareTo(ty.FullName);
        }

        public int Compare(IExpression x, IExpression y) =>
            (x, y) switch
            {
                (TypeTerm(Type tx), TypeTerm(Type ty)) => CompareTypes(tx, ty),
                _ => ExpressionOrdinalComparer.Instance.Compare(x, y)
            };

        public static readonly ExpressionTypeNarrowingComparer Instance =
            new ExpressionTypeNarrowingComparer();
    }
}
