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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Terms.Variable
{
    using static StaticFactories;

    [TestFixture]
    public sealed class LiteralTermTest
    {
        [Test]
        public void Literal1()
        {
            var context = Terrain.Create();

            /*
            Literal1:
            123
            123:_
            1:-------------------
            123:Numeric
            2:-------------------
            3:-------------------
            Numeric
             */

            var term = Literal(123);
            Assert.AreEqual("123:?", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("123:Numeric:*", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Literal2()
        {
            var context = Terrain.Create();

            /*
            Literal2:
            "ABC"
            "ABC":_
            1:-------------------
            "ABC":System.String
            2:-------------------
            3:-------------------
            System.String
             */

            var term = Literal("ABC");
            Assert.AreEqual("\"ABC\":?", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("\"ABC\":System.String:*", inferred.AnnotatedReadableString);
        }
    }
}
