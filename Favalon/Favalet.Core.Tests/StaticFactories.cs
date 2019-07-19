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

using Favalet.Terms;
using Favalet.Terms.Additionals;
using Favalet.Terms.Basis;
using Favalet.Terms.Internals;
using Favalet.Terms.Specialized;

namespace Favalet
{
    public static class StaticFactories
    {
        public static UnspecifiedTerm Unspecified =>
            Term.Unspecified;

        public static KindTerm Kind =>
            Term.Kind;
        public static TypeTerm Type =>
            Term.Type;

        public static LiteralTerm Literal(object value) =>
            Term.Literal(value, TextRange.Unknown);

        public static SymbolicVariableTerm Free(string name, Term higherOrder) =>
            Term.Free(name, higherOrder, TextRange.Unknown);
        public static SymbolicVariableTerm Free(string name) =>
            Term.Free(name, Unspecified, TextRange.Unknown);

        public static SymbolicVariableTerm Implicit(string name, Term higherOrder) =>
            ImplicitVariableTerm.Create(name, higherOrder, TextRange.Unknown);
        public static SymbolicVariableTerm Implicit(string name) =>
            ImplicitVariableTerm.Create(name, Unspecified, TextRange.Unknown);

        public static BoundVariableTerm Bound(string name, Term higherOrder) =>
            Term.Bound(name, higherOrder, TextRange.Unknown);
        public static BoundVariableTerm Bound(string name) =>
            Term.Bound(name, Unspecified, TextRange.Unknown);

        public static ApplyTerm Apply(Term function, Term argument, Term higherOrder) =>
            Term.Apply(function, argument, higherOrder, TextRange.Unknown);
        public static ApplyTerm Apply(Term function, Term argument) =>
            Term.Apply(function, argument, Unspecified, TextRange.Unknown);

        public static LambdaTerm Lambda(BoundVariableTerm parameter, Term term) =>
            Term.Lambda(parameter, term, TextRange.Unknown);
        public static LambdaTerm Lambda(LambdaTerm parameter, Term term) =>
            Term.Lambda(parameter, term, TextRange.Unknown);

        public static BindTerm Bind(BoundVariableTerm bound, Term term, Term higherOrder) =>
            Term.Bind(bound, term, higherOrder, TextRange.Unknown);
        public static BindTerm Bind(BoundVariableTerm bound, Term term) =>
            Term.Bind(bound, term, Unspecified, TextRange.Unknown);

        public static BindTerm RecursiveBind(BoundVariableTerm bound, Term term, Term higherOrder) =>
            Term.RecursiveBind(bound, term, higherOrder, TextRange.Unknown);
        public static BindTerm RecursiveBind(BoundVariableTerm bound, Term term) =>
            Term.RecursiveBind(bound, term, Unspecified, TextRange.Unknown);
    }
}
