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

using Favalet;
using Favalet.Terms;
using Favalet.Terms.Basis;
using Favalon.Internals;
using Favalon.Parsing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Favalon
{
    public static class Program
    {
        private static IEnumerable<string> wc(IEnumerable<string> stdin)
        {
            var bc = 0;
            var wc = 0;
            var lc = 0;

            foreach (var line in stdin)
            {
                bc += line.Length;
                wc += line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Length;
                lc++;
            }

            yield return $"{bc},{wc},{lc}";
        }

        public static async Task<int> Main(string[] args)
        {
            var terrain = Terrain.Create();

            var tystr = Term.Bound("System.String", Term.Kind, TextRange.Unknown);
            terrain.Bind(tystr, tystr);

            // wc = stdin:IE<s> -> stdin:IE<s>
            terrain.Bind(
                Term.Bound("wordcount", Term.Unspecified, TextRange.Unknown),
                Term.Lambda(
                    Term.Bound("stdin", tystr, TextRange.Unknown),
                    Term.Free("stdin", tystr, TextRange.Unknown),
                    TextRange.Unknown));

            // wc : System.String -> System.String
            // let r = wc "abc"

            // let r = "abc" | wc
            // let r = | "abc" wc

            // let | a b = b a
            // let | = fun a -> fun b -> b a

            // | = a -> b -> b a
            // | = a:IE<s> -> b:(IE<s> -> IE<s>) -> b a
            //terrain.Bind(
            //    Term.Bound("|", Term.Unspecified, TextRange.Unknown),
            //    Term.Lambda(
            //        Term.Bound("a", tystr, TextRange.Unknown),
            //        Term.Lambda(
            //            Term.Bound("b", Term.Lambda(tystr, tystr, TextRange.Unknown), TextRange.Unknown),
            //            Term.Apply(
            //                Term.Free("b", Term.Unspecified, TextRange.Unknown),
            //                Term.Free("a", Term.Unspecified, TextRange.Unknown),
            //                Term.Unspecified,
            //                TextRange.Unknown),
            //            TextRange.Unknown),
            //        TextRange.Unknown));

            // | = a:_ -> b:_ -> b a
            terrain.Bind(
                Term.Bound("|", Term.Unspecified, TextRange.Unknown),
                Term.Lambda(
                    Term.Bound("a", Term.Unspecified, TextRange.Unknown),
                    Term.Lambda(
                        Term.Bound("b", Term.Unspecified, TextRange.Unknown),
                        Term.Apply(
                            Term.Free("b", Term.Unspecified, TextRange.Unknown),
                            Term.Free("a", Term.Unspecified, TextRange.Unknown),
                            Term.Unspecified,
                            TextRange.Unknown),
                        TextRange.Unknown),
                    TextRange.Unknown));

            var console = InteractiveConsoleHost.Create();
            var parser = ObservableParser.Create(console);
            var abort = false;

            console.Write("Favalon> ");

            using (parser.Subscribe(new DelegatedObserver<ParseResult>(result =>
            {
                console.WriteLine();

                Term? WriteInfer(Term term)
                {
                    console.WriteLine($"parsed: {term.AnnotatedReadableString}");
                    var (inferred, errors) = terrain.Infer(term, Term.Unspecified);
                    foreach (var error in errors)
                    {
                        console.WriteLine($"{error}");
                    }
                    if (inferred != null)
                    {
                        console.WriteLine($"inferred: {inferred.AnnotatedReadableString}");
                    }
                    return inferred;
                }

                switch (result)
                {
                    case ParseResult(FreeVariableTerm term, _, _) when term.Name == "exit":
                        abort = true;
                        break;

                    case ParseResult(Term term, _, TextRange targetTextRange):
                        if (WriteInfer(term) is Term inferred)
                        {
                            var targetTerms = inferred.ExtractTermsByOverlaps(targetTextRange);
                        }
                        break;

                    case ParseResult(Term term, _, _):
                        WriteInfer(term);
                        break;

                    case ParseResult(_, ParseErrorInformation[] errors2, _):
                        foreach (var error in errors2)
                        {
                            console.WriteLine($"{error}");
                        }
                        break;
                }

                if (!abort)
                {
                    console.Write("Favalon> ");
                }
            })))
            {
                console.Run(ref abort);
            }

            return 0;
        }
    }
}
