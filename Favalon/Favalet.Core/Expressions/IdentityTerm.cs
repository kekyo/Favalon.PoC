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
    public interface IIdentityTerm : ITerm, IInferrableExpression
    {
        string Identity { get; }
    }

    public sealed class IdentityTerm : Term, IIdentityTerm
    {
        public readonly string Identity;

        private IdentityTerm(string identity, IExpression higherOrder)
        {
            this.Identity = identity;
            this.HigherOrder = higherOrder;
        }

        public override IExpression HigherOrder { get; }

        public IExpression Infer(IInferContext context)
        {
            if (context.Lookup(this) is IExpression expression)
            {
                return expression.InferIfRequired(context);
            }
            else
            {
                var higherOrder = this.HigherOrder.InferIfRequired(context);
                if (this.HigherOrder.Equals(higherOrder))
                {
                    return this;
                }
                else
                {
                    return new IdentityTerm(this.Identity, higherOrder);
                }
            }
        }

        string IIdentityTerm.Identity =>
            this.Identity;

        public override bool Equals(IExpression? rhs) =>
            rhs is IIdentityTerm identity &&
                this.Identity.Equals(identity.Identity);

        public override int GetHashCode() =>
            this.Identity.GetHashCode();

        public override string FormatString(IFormatStringContext context) =>
            context.Format(this, this.Identity);

        public static IdentityTerm Create(string identity, IExpression higherOrder) =>
            new IdentityTerm(identity, higherOrder);
    }
}
