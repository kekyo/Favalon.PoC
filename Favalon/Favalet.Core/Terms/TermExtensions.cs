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

using Favalet.Terms.Specialized;
using System.Collections.Generic;
using System.Linq;

namespace Favalet.Terms
{
    public static class TermExtensions
    {
        public static Term? Apply(this Term? function, Term? argument) =>
            (function, argument) switch
            {
                (Term f, Term a) => Term.Apply(f, a, UnspecifiedTerm.Instance, f.TextRange.Combine(a.TextRange)),
                (null, Term a) => a,
                (Term f, null) => f,
                _ => null
            };

        private static IEnumerable<Term> TraverseTermChildren(Term term) =>
            new[] { term }.
            Concat((term is ITraversableTerm traversableTerm ?
                traversableTerm.Children.Concat(new[] { term.HigherOrder }) :
                new[] { term.HigherOrder }).
                Where(t => t != null).
                SelectMany(TraverseTermChildren)).
            Where(t => !(t is ITraversableTerm));

        public static IEnumerable<Term> ExtractTermsByOverlaps(this Term term, TextRange targetTextRange) =>
            TraverseTermChildren(term).Where(t => t.TextRange.Overlaps(targetTextRange));
    }
}
