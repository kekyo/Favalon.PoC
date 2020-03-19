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

// Type overrides Object.Equals(object o) but does not override Object.GetHashCode()
#pragma warning disable CS0659

using Favalet.Contexts;
using System;

namespace Favalet.Expressions
{
    public interface IExpression : IEquatable<IExpression?>
    {
        IExpression HigherOrder { get; }

        string FormatString(IFormatStringContext context);
    }

    public abstract partial class Expression : IExpression
    {
        protected Expression()
        { }

        public abstract IExpression HigherOrder { get; }

        public abstract bool Equals(IExpression? rhs);

        public override sealed bool Equals(object obj) =>
            this.Equals(obj as IExpression);

        bool IEquatable<IExpression?>.Equals(IExpression? other) =>
            this.Equals(other);

        public abstract string FormatString(IFormatStringContext context);

        public sealed override string ToString() =>
            this.FormatString(NodeLabelledFormatStringContext.Instance);
    }
}
