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

namespace Favalet.Expressions
{
    public abstract partial class Expression
    {
        protected Expression(Expression higherOrder) =>
            this.HigherOrder = higherOrder;

        public readonly Expression HigherOrder;

        protected abstract Expression VisitInferring(IInferringEnvironment environment, Expression higherOrderHint);
        protected abstract Expression VisitResolving(IResolvingEnvironment environment);

        protected abstract FormattedString FormatReadableString(FormatContext context);

        public string ReadableString =>
            FormatReadableString(new FormatContext(
                FormatAnnotations.Standard, FormatNamings.Friendly, FormatOperators.Standard),
                this, false);
        public string StrictReadableString =>
            FormatReadableString(new FormatContext(
                FormatAnnotations.Always, FormatNamings.Standard, FormatOperators.Standard),
                this, false);

        public string FormatReadableString(
            FormatAnnotations formatAnnotation, FormatNamings formatNaming, FormatOperators formatOperator,
            bool encloseParenthesesIfRequired = false) =>
            FormatReadableString(new FormatContext(
                formatAnnotation, formatNaming, formatOperator), this, encloseParenthesesIfRequired);

        public override string ToString()
        {
            var name = this.GetType().Name.Replace("Expression", string.Empty);
            return $"{name}: {FormatReadableString(new FormatContext(FormatAnnotations.Standard, FormatNamings.Standard, FormatOperators.Standard), this, false)}";
        }
    }
}
