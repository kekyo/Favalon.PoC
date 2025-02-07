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
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Favalet.Expressions.Specialized
{
    public sealed class UnspecifiedTerm :
        Expression, ITerm, IInferrableExpression
    {
        private UnspecifiedTerm()
        { }

        public override IExpression HigherOrder =>
            TerminationTerm.Instance;

        public IExpression Infer(IInferContext context) =>
            context.CreatePlaceholder();

        IExpression IInferrableExpression.Fixup(IFixupContext context) =>
            this;

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public override bool Equals(IExpression? rhs, IEqualityComparer<IExpression> comparer) =>
            false;

        public override T Format<T>(IFormatContext<T> context) =>
            context.Format(this, FormatOptions.ForceText, "_");

        public static readonly UnspecifiedTerm Instance =
            new UnspecifiedTerm();
    }
}
