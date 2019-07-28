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

using Favalet.Terms.Basis;
using NUnit.Framework;
using System.Linq;

namespace Favalet.Terms
{
    [TestFixture]
    public sealed class ExtractionTest
    {
        [Test]
        public void ExtractFromPoint1()
        {
            var context = Terrain.Create();

            /*
            a1 -> b1 -> b1 a1
            01234567890123456
            */

            var term = Term.Lambda(
                Term.Bound("a1", Term.Unspecified, TextRange.Create("Extract", (0, 0, 0, 1))),
                Term.Lambda(
                    Term.Bound("b1", Term.Unspecified, TextRange.Create("Extract", (0, 6, 0, 7))),
                    Term.Apply(
                        Term.Free("b1", Term.Unspecified, TextRange.Create("Extract", (0, 12, 0, 13))),
                        Term.Free("a1", Term.Unspecified, TextRange.Create("Extract", (0, 15, 0, 16))),
                        Term.Unspecified,
                        TextRange.Create("Extract", (0, 12, 0, 16))),
                    TextRange.Create("Extract", (0, 6, 0, 7))),
                TextRange.Create("Extract", (0, 0, 0, 1)));

            var extracted = term.ExtractTermsByTextRange(TextRange.Create("Extract", (0, 12))).ToArray();

            Assert.AreEqual(1, extracted.Length);
            Assert.IsTrue(extracted[0] switch {
                FreeVariableTerm("b1", Term higherOrder, TextRange(_, Range(0, 12, 0, 13))) when (higherOrder == Term.Unspecified) => true,
                _ => false
            });
        }

        [Test]
        public void ExtractFromPoint2()
        {
            var context = Terrain.Create();

            /*
            a1 -> b1 -> b1 a1
            01234567890123456
            */

            var term = Term.Lambda(
                Term.Bound("a1", Term.Unspecified, TextRange.Create("Extract", (0, 0, 0, 1))),
                Term.Lambda(
                    Term.Bound("b1", Term.Unspecified, TextRange.Create("Extract", (0, 6, 0, 7))),
                    Term.Apply(
                        Term.Free("b1", Term.Unspecified, TextRange.Create("Extract", (0, 12, 0, 13))),
                        Term.Free("a1", Term.Unspecified, TextRange.Create("Extract", (0, 15, 0, 16))),
                        Term.Unspecified,
                        TextRange.Create("Extract", (0, 12, 0, 16))),
                    TextRange.Create("Extract", (0, 6, 0, 7))),
                TextRange.Create("Extract", (0, 0, 0, 1)));

            var extracted = term.ExtractTermsByTextRange(TextRange.Create("Extract", (0, 13))).ToArray();

            Assert.AreEqual(1, extracted.Length);
            Assert.IsTrue(extracted[0] switch
            {
                FreeVariableTerm("b1", Term higherOrder, TextRange(_, Range(0, 12, 0, 13))) when (higherOrder == Term.Unspecified) => true,
                _ => false
            });
        }
    }
}
