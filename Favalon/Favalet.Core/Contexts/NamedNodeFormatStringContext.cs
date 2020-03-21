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

using System.Linq;
using Favalet.Expressions;
using Favalet.Expressions.Specialized;
using Favalet.Internal;

namespace Favalet.Contexts
{
    public sealed class NamedNodeFormatStringContext : FormatStringContext
    {
        private readonly bool recursiveHigherOrder;

        private NamedNodeFormatStringContext(bool recursiveHigherOrder) =>
            this.recursiveHigherOrder = recursiveHigherOrder;

        private NamedNodeFormatStringContext(NamedNodeFormatStringContext parent, bool recursiveHigherOrder) :
            base(parent) =>
            this.recursiveHigherOrder = recursiveHigherOrder;

        public override string Format(IExpression expression, params object[] args)
        {
            var name = expression.GetType().Name;
            if (name.EndsWith("Expression"))
            {
                name = name.Substring(0, name.Length - "Expression".Length);
            }
            else if (name.EndsWith("Term"))
            {
                name = name.Substring(0, name.Length - "Term".Length);
            }

            var argsFormatted = StringUtilities.Join(
                ",",
                args.Select(arg => arg is IExpression ae ? ae.FormatString(this) : arg.ToString()));

            if (this.recursiveHigherOrder)
            {
                var higherOrder = expression.HigherOrder.
                    FormatString(expression.HigherOrder is PlaceholderTerm ?
                        this :
                        this.SuppressRecursive());
                return $"{name}({argsFormatted}):{higherOrder}";
            }
            else
            {
                return $"{name}({argsFormatted})";
            }
        }

        public override IFormatStringContext SuppressRecursive() =>
            new NamedNodeFormatStringContext(this, false);

        public static NamedNodeFormatStringContext Create() =>
            new NamedNodeFormatStringContext(true);
    }
}
