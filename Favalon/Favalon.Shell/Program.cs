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
using Favalon.Internals;
using Favalon.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;

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

        private static void InitializeTerrain(Terrain terrain)
        {
            foreach (var type in typeof(object).Assembly.GetTypes().
                Where(type => type.IsPublic && !type.IsNestedPublic && !type.IsGenericType))
            {
                var tystr = Term.Bound(type.FullName, Term.Kind, TextRange.Unknown);
                terrain.Bind(tystr, tystr);
            }
        }

        public static int Main(string[] args)
        {
            var console = InteractiveConsoleHost.Create();
            var parser = ObservableParser<InteractiveConsoleHost>.Create(console);

            using (var interpreter = ObservableInterpreter.Create(parser))
            {
                InitializeTerrain(interpreter.Terrain);

                var tystr = Term.Free("System.String", Term.Kind, TextRange.Unknown);

                // wc = stdin:IE<s> -> stdin:IE<s>
                interpreter.Terrain.Bind(
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
                //interpreter.Terrain.Bind(
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
                interpreter.Terrain.Bind(
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

                console.Run();
            }

            return 0;
        }
    }
}
