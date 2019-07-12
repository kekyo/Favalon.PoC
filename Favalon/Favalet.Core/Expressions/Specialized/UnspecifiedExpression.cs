// This is part of Favalon project.
// Copyright (c) 2019 Kouji Matsui
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;

namespace Favalet.Expressions.Specialized
{
    public sealed class UnspecifiedExpression : Expression
    {
        private UnspecifiedExpression(Expression higherOrder) :
            base(higherOrder)
        { }

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (context.FormatNaming == FormatNamings.Strict) ? "(Unspecified)" : "_";

        protected override Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint) =>
            throw new NotImplementedException();

        protected override Expression VisitResolving(IResolvingEnvironment environment) =>
            this;

        internal static readonly UnspecifiedExpression Instance =
            new UnspecifiedExpression(null!);
        internal static new readonly UnspecifiedExpression Kind =
            new UnspecifiedExpression(KindExpression.Instance);
        internal static new readonly UnspecifiedExpression Type =
            new UnspecifiedExpression(TypeExpression.Instance);
    }
}
