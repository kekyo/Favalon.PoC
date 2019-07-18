// This is part of Favalon project - https://github.com/kekyo/Favalon
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

using Favalet.Expressions.Specialized;
using System;
using System.Reflection;

namespace Favalet.Expressions.Additionals
{
    public sealed class LiteralExpression : ValueExpression, IEquatable<LiteralExpression?>
    {
        internal LiteralExpression(object value, Expression higherOrder, TextRange textRange) :
            base(higherOrder, textRange) =>
            this.Value = value;

        public readonly object Value;

        protected override FormattedString FormatReadableString(FormatContext context) =>
            (this.Value is string) ? $"\"{this.Value}\"" : this.Value.ToString();

        private string GetTypeName()
        {
#if NETSTANDARD1_0
            var type = this.Value.GetType().GetTypeInfo();
#else
            var type = this.Value.GetType();
#endif
            if (type.IsPrimitive)
            {
                return "Numeric";
            }
            else
            {
                return type.FullName;
            }
        }

        protected override Expression VisitInferring(IInferringContext context, Expression higherOrderHint)
        {
            var typedHigherOrder = new FreeVariableExpression(this.GetTypeName(), KindExpression.Instance, this.TextRange);
            var higherOrder = context.Unify(typedHigherOrder, higherOrderHint);

            return new LiteralExpression(this.Value, higherOrder, this.TextRange);
        }

        protected override Expression VisitResolving(IResolvingContext context)
        {
            var higherOrder = context.Visit(this.HigherOrder);
            return new LiteralExpression(this.Value, higherOrder, this.TextRange);
        }

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public bool Equals(LiteralExpression? other) =>
            other?.Value.Equals(this.Value) ?? false;

        public override bool Equals(object obj) =>
            this.Equals(obj as LiteralExpression);
    }
}
