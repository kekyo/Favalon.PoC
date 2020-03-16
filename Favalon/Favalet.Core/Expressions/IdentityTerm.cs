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

namespace Favalet.Expressions
{
    public interface IIdentityTerm : ITerm
    {
        public string Identity { get; }
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

        string IIdentityTerm.Identity =>
            this.Identity;

        public override bool Equals(IExpression? rhs) =>
            rhs is IIdentityTerm identity &&
                this.Identity.Equals(identity.Identity);

        public static IdentityTerm Create(string identity, IExpression higherOrder) =>
            new IdentityTerm(identity, higherOrder);
    }
}
