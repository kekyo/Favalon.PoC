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

using Favalet.Contexts;

namespace Favalet.Expressions
{
    public interface IConstantTerm : ITerm
    {
        object Value { get; }
    }

    public sealed class ConstantTerm : Term, IConstantTerm
    {
        public readonly object Value;

        private ConstantTerm(object value) =>
            this.Value = value;

        public override IExpression HigherOrder =>
            TypeTerm.From(this.Value.GetType());

        object IConstantTerm.Value =>
            this.Value;

        public override bool Equals(IExpression? rhs) =>
            rhs is IConstantTerm constant &&
                this.Value.Equals(constant.Value);

        public override int GetHashCode() =>
            this.Value.GetHashCode();

        public override string FormatString(IFormatStringContext context) =>
            this.Value switch
            {
                string str => $"\"{str}\"",
                char ch => $"'{ch}'",
                _ => this.Value.ToString()
            };

        public static IConstantTerm From(object value) =>
            new ConstantTerm(value);
    }
}
