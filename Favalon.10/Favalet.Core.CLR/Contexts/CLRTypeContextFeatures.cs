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

using Favalet.Contexts;
using Favalet.Expressions.Algebraic;
using System;
using System.Collections.Generic;

namespace Favalet.Expressions
{
    public sealed class CLRTypeContextFeatures : TypeContextFeatures
    {
        private CLRTypeContextFeatures()
        { }

        public override IExpression CreateNumeric(string value) =>
            CLRExpressionFactory.FromNumeric(value);

        public override IExpression CreateString(string value) =>
            ConstantTerm.From(value);

        public override WidenedResult Widen(
            IExpression to, IExpression from,
            Func<IEnumerable<IExpression>, IExpression?> createOr,
            Func<IExpression, IExpression, WidenedResult> widen)
        {
            switch (to, from)
            {
                // object: object <-- int
                // double: double <-- int
                // IComparable: IComparable <-- string
                case (TypeTerm toType, TypeTerm fromType):
                    return toType.IsConvertibleFrom(fromType) ?
                        WidenedResult.Success(to) :
                        WidenedResult.Empty;

                //case (MethodTerm toMethod, MethodTerm fromMethod):
                //    return (widen(toMethod.HigherOrder, fromMethod.HigherOrder) != null) ?
                //        toMethod :
                //        null;

                default:
                    return base.Widen(to, from, createOr, widen);
            }
        }

        public static new readonly CLRTypeContextFeatures Instance =
            new CLRTypeContextFeatures();
    }
}
