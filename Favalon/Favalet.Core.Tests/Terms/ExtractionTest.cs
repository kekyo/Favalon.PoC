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

using NUnit.Framework;
using System.Linq;

namespace Favalet.Terms
{
    [TestFixture]
    public sealed class ExtractionTest
    {
        [Test]
        public void Extract()
        {
            var context = Terrain.Create();

            /*
            a -> b -> b a
            0123456789012
            */

            var term = Term.Lambda(
                Term.Bound("a", Term.Unspecified, TextRange.Create("Extract", (0, 0, 0, 0))),
                Term.Lambda(
                    Term.Bound("b", Term.Unspecified, TextRange.Create("Extract", (0, 5, 0, 5))),
                    Term.Apply(
                        Term.Free("b", Term.Unspecified, TextRange.Create("Extract", (0, 10, 0, 10))),
                        Term.Free("a", Term.Unspecified, TextRange.Create("Extract", (0, 12, 0, 12))),
                        Term.Unspecified,
                        TextRange.Create("Extract", (0, 10, 0, 12))),
                    TextRange.Create("Extract", (0, 5, 0, 12))),
                TextRange.Create("Extract", (0, 0, 0, 12)));

            var before1 = term.ExtractTermsByTextRange(TextRange.Create("Extract", (0, 0, 0, 0))).ToArray();
            var before2 = term.ExtractTermsByTextRange(TextRange.Create("Extract", (0, 5, 0, 5))).ToArray();
            var before3 = term.ExtractTermsByTextRange(TextRange.Create("Extract", (0, 10, 0, 12))).ToArray();

            var (inferred, _) = context.Infer(term, Term.Unspecified);

            var after1 = inferred!.ExtractTermsByTextRange(TextRange.Create("Extract", (0, 0, 0, 0))).ToArray();
            var after2 = inferred!.ExtractTermsByTextRange(TextRange.Create("Extract", (0, 5, 0, 5))).ToArray();
            var after3 = inferred!.ExtractTermsByTextRange(TextRange.Create("Extract", (0, 10, 0, 12))).ToArray();
        }
    }
}
