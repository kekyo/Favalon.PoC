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

using Favalet.Terms.Additionals;
using Favalet.Terms.Basis;
using Favalet.Terms.Specialized;

namespace Favalet.Terms
{
    partial class Term
    {
        public static UnspecifiedTerm Unspecified =>
            UnspecifiedTerm.Instance;

        public static KindTerm Kind =>
            KindTerm.Instance;
        public static TypeTerm Type =>
            TypeTerm.Instance;

        public static LiteralTerm Literal(object value, TextRange textRange) =>
            new LiteralTerm(value, TypeTerm.Instance, textRange);

        public static SymbolicVariableTerm Free(string name, Term higherOrder, TextRange textRange) =>
            new FreeVariableTerm(name, higherOrder, textRange);

        public static BoundVariableTerm Bound(string name, Term higherOrder, TextRange textRange) =>
            new BoundVariableTerm(name, higherOrder, textRange);

        public static ApplyTerm Apply(Term function, Term argument, Term higherOrder, TextRange textRange) =>
            new ApplyTerm(function, argument, higherOrder, textRange);

        public static LambdaTerm Lambda(BoundVariableTerm parameter, Term term, TextRange textRange) =>
            LambdaTerm.Create(parameter, term, false, textRange);
        public static LambdaTerm Lambda(LambdaTerm parameter, Term term, TextRange textRange) =>
            LambdaTerm.Create(parameter, term, false, textRange);

        public static BindTerm Bind(BoundVariableTerm bound, Term term, Term higherOrder, TextRange textRange) =>
            new BindTerm(bound, term, false, higherOrder, textRange);

        public static BindTerm RecursiveBind(BoundVariableTerm bound, Term term, Term higherOrder, TextRange textRange) =>
            new BindTerm(bound, term, true, higherOrder, textRange);

        public static Term? operator +(Term? lhs, Term? rhs) =>
            lhs.Apply(rhs);
    }
}
