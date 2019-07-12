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

namespace Favalet.Expressions.Variable
{
    using static StaticFactories;

    [TestFixture]
    public sealed class LambdaExpressionTest
    {
        [Test]
        public void Lambda1()
        {
            var environment = Environment.Create();

            /*
            Lambda 1:
            a -> a
            (a:? -> a:?):_
            1:-------------------
            (a:? -> a:?):'1:_
            (a:'2:* -> a:?):'1:_                           : Bind(a:'2)
            (a:'2:* -> a:'2:*):'1:_                        : Lookup(a => '2), Memoize('1 => ('2 -> '2))
            2:-------------------
            (a:'2:* -> a:'2:*):('2:* -> '2:*):_            : Update('1 => ('2 -> '2))
            3:-------------------
            '2:* -> '2:*
            */

            var expression = Lambda(Bound("a", Type), Free("a", Type));
            Assert.AreEqual("(a:? -> a:?):_", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:'2:* -> a:'2:*):('2:* -> '2:*):(* -> *)", inferred.StrictReadableString);
        }
#if false
        [Test]
        public void Lambda2()
        {
            var environment = Environment.Create();

            /*
            Lambda 2:
            a -> a:System.Int32
            (a:_ -> a:System.Int32):_
            1:-------------------
            (a:_ -> a:System.Int32):'1
            (a:'2 -> a:System.Int32):'1          : Bind(a:'2)
            (a:'2 -> a:System.Int32):'1          : Lookup(a => '2), Memoize('2 => System.Int32), Memoize('1 => ('2 -> System.Int32))
            2:-------------------
            (a:System.Int32 -> a:System.Int32):'1          : Update('2 => System.Int32)
            (a:System.Int32 -> a:System.Int32):('2 -> System.Int32)          : Update('1 => ('2 -> System.Int32))
            (a:System.Int32 -> a:System.Int32):(System.Int32 -> System.Int32)          : Update('2 => System.Int32)
            3:-------------------
            System.Int32 -> System.Int32
            */

            var expression = Lambda(Bound("a"), Free("a", Free("System.Int32")));
            Assert.AreEqual("(a:_ -> a:System.Int32:_):_", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:System.Int32:_ -> a:System.Int32:_):(System.Int32:_ -> System.Int32:_):(_ -> _):_", inferred.StrictReadableString);
        }

        [Test]
        public void Lambda3()
        {
            var environment = Environment.Create();

            /*
            Lambda 3:
            a:System.Int32 -> a
            (a:System.Int32 -> a:_):_
            1:-------------------
            (a:System.Int32 -> a:_):'1
            (a:System.Int32 -> a:_):'1                      : Bind(a:System.Int32)
            (a:System.Int32 -> a:System.Int32):'1           : Lookup(a => System.Int32), Memoize('1 => (System.Int32 -> System.Int32))
            2:-------------------
            (a:System.Int32 -> a:System.Int32):(System.Int32 -> System.Int32)           : Update('1 => (System.Int32 -> System.Int32))
            3:-------------------
            System.Int32 -> System.Int32
            */

            var expression = Lambda(Bound("a", Free("System.Int32")), Free("a"));
            Assert.AreEqual("(a:System.Int32:_ -> a:_):_", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:System.Int32:_ -> a:System.Int32:_):(System.Int32:_ -> System.Int32:_):(_ -> _):_", inferred.StrictReadableString);
        }

        [Test]
        public void Lambda4()
        {
            var environment = Environment.Create();

            /*
            Lambda 4:
            a -> b -> a
            (a:_ -> (b:_ -> a:_):_):_
            1:-------------------
            (a:_ -> (b:_ -> a:_):_):'1
            (a:'2 -> (b:_ -> a:_):_):'1                     : Bind(a:'2)
            (a:'2 -> (b:_ -> a:_):'3):'1                    : Memoize('1 => ('2 -> '3))
            (a:'2 -> (b:'4 -> a:_):'3):'1                   : Bind(b:'4)
            (a:'2 -> (b:'4 -> a:'2):'3):'1                  : Lookup(a => '2), Memoize('3 => ('4 -> '2))
            2:-------------------
            (a:'2 -> (b:'4 -> a:'2):('4 -> '2)):'1          : Update('3 => ('4 -> '2))
            (a:'2 -> (b:'4 -> a:'2):('4 -> '2)):('2 -> '3)  : Update('1 => ('2 -> '3))
            (a:'2 -> (b:'4 -> a:'2):('4 -> '2)):('2 -> ('4 -> '2))      : Update('3 => ('4 -> '2))
            3:-------------------
            '2 -> ('4 -> '2)
            '2 -> '4 -> '2
            */

            var expression = Lambda(Bound("a"), Lambda(Bound("b"), Free("a")));
            Assert.AreEqual("(a:_ -> (b:_ -> a:_):_):_", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:'2:_ -> (b:'4:_ -> a:'2:_):('4:_ -> '2:_):(_ -> _):_):('2:_ -> ('4:_ -> '2:_):(_ -> _):_):(_ -> (_ -> _):_):_", inferred.StrictReadableString);
        }

        [Test]
        public void Lambda5()
        {
            var environment = Environment.Create();

            /*
            Lambda 5:
            a -> b -> b
            (a:_ -> (b:_ -> b:_):_):_
            1:-------------------
            (a:_ -> (b:_ -> b:_):_):'1
            (a:'2 -> (b:_ -> b:_):_):'1                     : Bind(a:'2)
            (a:'2 -> (b:_ -> b:_):'3):'1                    : Memoized('1 => ('2 -> '3))
            (a:'2 -> (b:'4 -> b:_):'3):'1                   : Bind(b:'4)
            (a:'2 -> (b:'4 -> b:'4):'3):'1                  : Lookup(b => '4), Memoized('3 => ('4 -> '4))
            2:-------------------
            (a:'2 -> (b:'4 -> b:'4):('4 -> '4)):'1          : Update('3 => ('4 -> '4))
            (a:'2 -> (b:'4 -> b:'4):('4 -> '4)):('2 -> '3)  : Update('1 => ('2 -> '3))
            (a:'2 -> (b:'4 -> b:'4):('4 -> '4)):('2 -> ('4 -> '4))      : Update('3 => ('4 -> '4))
            3:-------------------
            '2 -> ('4 -> '4)
            '2 -> '4 -> '4
            */

            var expression = Lambda(Bound("a"), Lambda(Bound("b"), Free("b")));
            Assert.AreEqual("(a:_ -> (b:_ -> b:_):_):_", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:'2:_ -> (b:'4:_ -> b:'4:_):('4:_ -> '4:_):(_ -> _):_):('2:_ -> ('4:_ -> '4:_):(_ -> _):_):(_ -> (_ -> _):_):_", inferred.StrictReadableString);
        }

        [Test]
        public void Lambda6()
        {
            var environment = Environment.Create();

            /*
            Lambda 6:
            a -> b -> a:System.Int32
            (a:_ -> (b:_ -> a:System.Int32):_):_
            1:-------------------
            (a:_ -> (b:_ -> a:System.Int32):_):'1
            (a:'2 -> (b:_ -> a:System.Int32):_):'1          : Bind(a:'2)
            (a:'2 -> (b:_ -> a:System.Int32):'3):'1         : Memoized('1 => ('2 -> '3))
            (a:'2 -> (b:'4 -> a:System.Int32):'3):'1        : Bind(b:'4)
            (a:'2 -> (b:'4 -> a:System.Int32):'3):'1        : Lookup(a => '2), Memoized('2 => System.Int32)
            (a:'2 -> (b:'4 -> a:System.Int32):'3):'1        : Memoized('3 => ('4 -> System.Int32))
            2:-------------------
            (a:'2 -> (b:'4 -> a:System.Int32):('4 -> System.Int32)):'1        : Update('3 => ('4 -> System.Int32))
            (a:System.Int32 -> (b:'4 -> a:System.Int32):('4 -> System.Int32)):'1        : Update('2 => System.Int32)
            (a:System.Int32 -> (b:'4 -> a:System.Int32):('4 -> System.Int32)):('2 -> '3)        : Update('1 => ('2 -> '3))
            (a:System.Int32 -> (b:'4 -> a:System.Int32):('4 -> System.Int32)):(System.Int32 -> '3)        : Update('2 => System.Int32)
            (a:System.Int32 -> (b:'4 -> a:System.Int32):('4 -> System.Int32)):(System.Int32 -> ('4 -> System.Int32))        : Update('3 => ('4 -> System.Int32))
            3:-------------------
            System.Int32 -> ('4 -> System.Int32)
            System.Int32 -> '4 -> System.Int32
            */

            var expression = Lambda(Bound("a"), Lambda(Bound("b"), Free("a", Free("System.Int32"))));
            Assert.AreEqual("(a:_ -> (b:_ -> a:System.Int32:_):_):_", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:System.Int32:_ -> (b:'4:_ -> a:System.Int32:_):('4:_ -> System.Int32:_):(_ -> _):_):(System.Int32:_ -> ('4:_ -> System.Int32:_):(_ -> _):_):(_ -> (_ -> _):_):_", inferred.StrictReadableString);
        }

        [Test]
        public void Lambda7()
        {
            var environment = Environment.Create();

            /*
            Lambda 7:
            a -> b:System.Int32 -> a
            (a:_ -> (b:System.Int32 -> a:_):_):_
            1:-------------------
            (a:_ -> (b:System.Int32 -> a:_):_):'1
            (a:'2 -> (b:System.Int32 -> a:_):_):'1           : Bind(a:'2)
            (a:'2 -> (b:System.Int32 -> a:_):'3):'1          : Memoize('1 => ('2 -> '3))
            (a:'2 -> (b:System.Int32 -> a:_):'3):'1          : Bind(b:System.Int32)
            (a:'2 -> (b:System.Int32 -> a:'2):'3):'1         : Lookup(a => '2), Memoize('3 => (System.Int32 -> '2))
            2:-------------------
            (a:'2 -> (b:System.Int32 -> a:'2):(System.Int32 -> '2)):'1           : Update('3 => (System.Int32 -> '2))
            (a:'2 -> (b:System.Int32 -> a:'2):(System.Int32 -> '2)):('2 -> '3)             : Update('1 => ('2 -> '3))
            (a:'2 -> (b:System.Int32 -> a:'2):(System.Int32 -> '2)):('2 -> (System.Int32 -> '2))         : Update('3 => (System.Int32 -> '2))
            3:-------------------
            '2 -> (System.Int32 -> '2)
            '2 -> System.Int32 -> '2
            */

            var expression = Lambda(Bound("a"), Lambda(Bound("b", Free("System.Int32")), Free("a")));
            Assert.AreEqual("(a:_ -> (b:System.Int32:_ -> a:_):_):_", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:'2:_ -> (b:System.Int32:_ -> a:'2:_):(System.Int32:_ -> '2:_):(_ -> _):_):('2:_ -> (System.Int32:_ -> '2:_):(_ -> _):_):(_ -> (_ -> _):_):_", inferred.StrictReadableString);
        }

        [Test]
        public void Lambda8()
        {
            var environment = Environment.Create();

            /*
            Lambda 8:
            a:System.Int32 -> b -> a
            (a:System.Int32 -> (b:_ -> a:_):_):_
            1:-------------------
            (a:System.Int32 -> (b:_ -> a:_):_):'1
            (a:System.Int32 -> (b:_ -> a:_):_):'1             : Bind(a:System.Int32)
            (a:System.Int32 -> (b:_ -> a:_):'2):'1            : Memoize('1 => (System.Int32 -> '2))
            (a:System.Int32 -> (b:'3 -> a:_):'2):'1           : Bind(b:'3)
            (a:System.Int32 -> (b:'3 -> a:System.Int32):'2):'1           : Lookup(a:System.Int32), Memoize('2 => ('3 -> System.Int32))
            2:-------------------
            (a:System.Int32 -> (b:'3 -> a:System.Int32):('3 -> System.Int32)):'1           : Update('2 => ('3 -> System.Int32))
            (a:System.Int32 -> (b:'3 -> a:System.Int32):('3 -> System.Int32)):(System.Int32 -> '2)           : Update('1 => (System.Int32 -> '2))
            (a:System.Int32 -> (b:'3 -> a:System.Int32):('3 -> System.Int32)):(System.Int32 -> ('3 -> System.Int32))           : Update('2 => ('3 -> System.Int32))
            3:-------------------
            System.Int32 -> ('3 -> System.Int32)
            System.Int32 -> '3 -> System.Int32
            */

            var expression = Lambda(Bound("a", Free("System.Int32")), Lambda(Bound("b"), Free("a")));
            Assert.AreEqual("(a:System.Int32:_ -> (b:_ -> a:_):_):_", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:System.Int32:_ -> (b:'3:_ -> a:System.Int32:_):('3:_ -> System.Int32:_):(_ -> _):_):(System.Int32:_ -> ('3:_ -> System.Int32:_):(_ -> _):_):(_ -> (_ -> _):_):_", inferred.StrictReadableString);
        }
#endif
    }
}