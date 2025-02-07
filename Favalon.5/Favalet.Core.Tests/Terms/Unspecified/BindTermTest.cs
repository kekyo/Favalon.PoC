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

namespace Favalet.Terms.Unspecified
{
    using static StaticFactories;

    [TestFixture]
    public sealed class BindTermTest
    {
        [Test]
        public void Bind1()
        {
            var context = Terrain.Create();

            /*
            Bind 1:
            a = 123
            (a:_ = 123:_):_
            1:-------------------
            (a:_ = 123:_):'1
            (a:_ = 123:Numeric):'1                       : Memoize('1 => Numeric)
            (a:Numeric = 123:Numeric):'1
            2:-------------------
            (a:Numeric = 123:Numeric):Numeric            : Update('1 => Numeric)
            3:-------------------
            Numeric
            */

            var term = Bind(Bound("a"), Literal(123));
            Assert.AreEqual("(a:_ = 123:?):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("(a:Numeric:* = 123:Numeric:*):Numeric:*", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Bind2()
        {
            var context = Terrain.Create();

            /*
            Bind 2:
            a = b -> b
            (a:_ = (b:_ -> b:_):_):_
            1:-------------------
            (a:_ = (b:_ -> b:_):_):'1
            (a:_ = (b:_ -> b:_):'1):'1
            (a:_ = (b:'2 -> b:_):'1):'1                  : Bind(b:'2)
            (a:_ = (b:'2 -> b:'2):'1):'1                 : Lookup(b => '2), Memoize('1 => ('2 -> '2))
            (a:'1 = (b:'2 -> b:'2):'1):'1
            2:-------------------
            (a:'1 = (b:'2 -> b:'2):('2 -> '2)):'1                        : Update('1 => ('2 -> '2))
            (a:('2 -> '2) = (b:'2 -> b:'2):('2 -> '2)):'1                : Update('1 => ('2 -> '2))
            (a:('2 -> '2) = (b:'2 -> b:'2):('2 -> '2)):('2 -> '2)        : Update('1 => ('2 -> '2))
            3:-------------------
            '2 -> '2
            */

            var term = Bind(Bound("a"), Lambda(Bound("b"), Free("b")));
            Assert.AreEqual("(a:_ = (b:_ -> b:_):_):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("(a:('a -> 'a) = (b:'a -> b:'a):('a -> 'a)):('a -> 'a)", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Bind3()
        {
            var context = Terrain.Create();

            /*
            Bind 3:
            a = b -> b:System.Int32
            (a:_ = (b:_ -> b:System.Int32):_):_
            1:-------------------
            (a:_ = (b:_ -> b:System.Int32):_):'1
            (a:'1 = (b:_ -> b:System.Int32):_):'1
            (a:'1 = (b:_ -> b:System.Int32):'1):'1
            (a:'1 = (b:'2 -> b:System.Int32):'1):'1      : Bind(b:'2)
            (a:'1 = (b:'2 -> b:System.Int32):'1):'1      : Lookup(b => '2), Memoize('2 => System.Int32)
            2:-------------------
            (a:'1 = (b:System.Int32 -> b:System.Int32):'1):'1      : Update('2 => System.Int32), Memoize('1 => (System.Int32 -> System.Int32))
            (a:'1 = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1      : Update('1 => (System.Int32 -> System.Int32))
            (a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1      : Update('1 => (System.Int32 -> System.Int32))
            (a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)      : Update('1 => (System.Int32 -> System.Int32))
            3:-------------------
            System.Int32 -> System.Int32
            */

            var term = Bind(Bound("a"), Lambda(Bound("b"), Free("b", Implicit("System.Int32"))));
            Assert.AreEqual("(a:_ = (b:_ -> b:System.Int32:_):_):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("(a:(System.Int32:'a -> System.Int32:'a):('a -> 'a) = (b:System.Int32:'a -> b:System.Int32:'a):(System.Int32:'a -> System.Int32:'a):('a -> 'a)):(System.Int32:'a -> System.Int32:'a):('a -> 'a)", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Bind4()
        {
            var context = Terrain.Create();

            /*
            Bind 4:
            a = b:System.Int32 -> b
            (a:_ = (b:System.Int32 -> b:_):_):_
            1:-------------------
            (a:_ = (b:System.Int32 -> b:_):_):'1
            (a:'1 = (b:System.Int32 -> b:_):_):'1
            (a:'1 = (b:System.Int32 -> b:_):'1):'1
            (a:'1 = (b:System.Int32 -> b:_):'1):'1                   : Bind(b:System.Int32)
            (a:'1 = (b:System.Int32 -> b:System.Int32):'1):'1        : Lookup(b => System.Int32), Memoize('1 => (System.Int32 -> System.Int32))
            2:-------------------
            (a:'1 = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1        : Update('1 => (System.Int32 -> System.Int32))
            (a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1        : Update('1 => (System.Int32 -> System.Int32))
            (a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)        : Update('1 => (System.Int32 -> System.Int32))
            3:-------------------
            System.Int32 -> System.Int32
            */

            var term = Bind(Bound("a"), Lambda(Bound("b", Implicit("System.Int32")), Free("b")));
            Assert.AreEqual("(a:_ = (b:System.Int32:_ -> b:_):_):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("(a:(System.Int32:'a -> System.Int32:'a):('a -> 'a) = (b:System.Int32:'a -> b:System.Int32:'a):(System.Int32:'a -> System.Int32:'a):('a -> 'a)):(System.Int32:'a -> System.Int32:'a):('a -> 'a)", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Bind5()
        {
            var context = Terrain.Create();

            /*
            Bind 5:
            a:System.Int32 = b -> b
            (a:System.Int32 = (b:_ -> b:_):_):_
            1:-------------------
            (a:System.Int32 = (b:_ -> b:_):_):'1
            (a:System.Int32 = (b:_ -> b:_):'1):'1
            (a:System.Int32 = (b:'2 -> b:_):'1):'1                      : Bind(b:'2)
            (a:System.Int32 = (b:'2 -> b:'2):('2 -> '2)):'1             : Lookup(b => '2)
            (a:System.Int32 = (b:'2 -> b:'2):('2 -> '2)):'1
            (a:System.Int32 = (b:'2 -> b:'2):('2 -> '2)):'1             : Unification problem (('2 -> '2) => System.Int32)
            */

            var term = Bind(Bound("a", Implicit("System.Int32")), Lambda(Bound("b"), Free("b")));
            Assert.AreEqual("(a:System.Int32:_ = (b:_ -> b:_):_):_", term.StrictReadableString);

            try
            {
                context.Infer(term, Term.Unspecified);
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Cannot unify: between \"('2:_ -> '2:_):(_ -> _)\" and \"System.Int32:_\"", ex.Message);
            }
        }

        [Test]
        public void Bind6()
        {
            var context = Terrain.Create();

            /*
            Bind 6:
            a:(System.Int32 -> _) = b -> b
            (a:(System.Int32 -> _) = (b:_ -> b:_):_):_
            1:-------------------
            (a:(System.Int32 -> _) = (b:_ -> b:_):_):'1
            (a:(System.Int32 -> '2) = (b:_ -> b:_):_):'1        : Memoize('1 => (System.Int32 -> '2))
            (a:(System.Int32 -> '2) = (b:_ -> b:_):(System.Int32 -> '2)):'1
            (a:(System.Int32 -> '2) = (b:System.Int32 -> b:_):(System.Int32 -> '2)):'1      : Bind(b:System.Int32)
            (a:(System.Int32 -> '2) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> '2)):'1     : Lookup(b => System.Int32), Memoize((System.Int32 -> '2) => (System.Int32 -> System.Int32))
            2:-------------------
            (a:(System.Int32 -> '2) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1     : Update((System.Int32 -> '2) => (System.Int32 -> System.Int32))
            (a:(System.Int32 -> '2) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> '2)       : Update('1 => (System.Int32 -> '2))
            (a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> '2)       : Update((System.Int32 -> '2) => (System.Int32 -> System.Int32))
            (a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)       : Update((System.Int32 -> '2) => (System.Int32 -> System.Int32))
            3:-------------------
            System.Int32 -> System.Int32
            */

            var term = Bind(Bound("a", Lambda(Bound("System.Int32"), Unspecified)), Lambda(Bound("b"), Free("b")));
            Assert.AreEqual("(a:(System.Int32:_ -> _):_ = (b:_ -> b:_):_):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("(a:(System.Int32:'a -> System.Int32:'a) = (b:System.Int32:'a -> b:System.Int32:'a):(System.Int32:'a -> System.Int32:'a):('a -> 'a)):(System.Int32:'a -> System.Int32:'a)", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Bind7()
        {
            var context = Terrain.Create();

            /*
            a = b -> a
            (a:_ = (b:_ -> a:_):_):_
            1:-------------------
            (a:_ = (b:_ -> a:_):_):'1
            (a:'1 = (b:_ -> a:_):_):'1                  : Bind(a:'1)
            (a:'1 = (b:_ -> a:_):'1):'1
            (a:'1 = (b:'2 -> a:_):'1):'1                : Bind(b:'2)
            (a:'1 = (b:'2 -> a:'1):'1):'1               : Lookup(a => '1), Memoize('1 => ('2 -> '1))
            2:-------------------
            (a:'1 = (b:'2 -> a:'1):('2 -> '1)):'1       : Update('1 => ('2 -> '1))
            (a:('2 -> '1) = (b:'2 -> a:'1):('2 -> '1)):'1       : Update('1 => ('2 -> '1))     // Recursive unification problem ('1 => ('2 -> '1))
            */

            var term = Bind(Bound("a"), Lambda(Bound("b"), Free("a")));
            Assert.AreEqual("(a:_ = (b:_ -> a:_):_):_", term.StrictReadableString);

            try
            {
                context.Infer(term, Term.Unspecified);
                Assert.Fail();
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual("Cannot find variable: Name=a", ex.Message);
            }
        }
    }
}
