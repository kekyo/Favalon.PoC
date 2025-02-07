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
    public sealed class RecursiveBindTermTest
    {
        [Test]
        public void RecursiveBind1()
        {
            var context = Terrain.Create();

            /*
            Recursive bind 1:
            rec a = 123
            (rec a:_ = 123:_):_
            1:-------------------
            (rec a:_ = 123:_):'1
            (rec a:'1 = 123:Numeric):'1                      : Bind(a:'1)
            (rec a:'1 = 123:Numeric):'1                      : Memoize('1 => Numeric)
            2:-------------------
            (rec a:Numeric = 123:Numeric):'1                 : Update('1 => Numeric)
            (rec a:Numeric = 123:Numeric):Numeric            : Update('1 => Numeric)
            3:-------------------
            Numeric
            */

            var term = RecursiveBind(Bound("a"), Literal(123));
            Assert.AreEqual("(rec a:_ = 123:?):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("(rec a:Numeric:* = 123:Numeric:*):Numeric:*", inferred.AnnotatedReadableString);
        }

        [Test]
        public void RecursiveBind2()
        {
            var context = Terrain.Create();

            /*
            Recursive bind 2:
            rec a = b -> b
            (rec a:_ = (b:_ -> b:_):_):_
            1:-------------------
            (rec a:_ = (b:_ -> b:_):_):'1
            (rec a:_ = (b:_ -> b:_):'1):'1
            (rec a:_ = (b:'2 -> b:_):'1):'1                  : Bind(b:'2)
            (rec a:_ = (b:'2 -> b:'2):'1):'1                 : Lookup(b => '2), Memoize('1 => ('2 -> '2))
            (rec a:'1 = (b:'2 -> b:'2):'1):'1
            2:-------------------
            (rec a:'1 = (b:'2 -> b:'2):('2 -> '2)):'1                        : Update('1 => ('2 -> '2))
            (rec a:('2 -> '2) = (b:'2 -> b:'2):('2 -> '2)):'1                : Update('1 => ('2 -> '2))
            (rec a:('2 -> '2) = (b:'2 -> b:'2):('2 -> '2)):('2 -> '2)        : Update('1 => ('2 -> '2))
            3:-------------------
            '2 -> '2
            */

            var term = RecursiveBind(Bound("a"), Lambda(Bound("b"), Free("b")));
            Assert.AreEqual("(rec a:_ = (b:_ -> b:_):_):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("(rec a:('a -> 'a) = (b:'a -> b:'a):('a -> 'a)):('a -> 'a)", inferred.AnnotatedReadableString);
        }

        [Test]
        public void RecursiveBind3()
        {
            var context = Terrain.Create();

            /*
            Recursive bind 3:
            rec a = b -> b:System.Int32
            (rec a:_ = (b:_ -> b:System.Int32):_):_
            1:-------------------
            (rec a:_ = (b:_ -> b:System.Int32):_):'1
            (rec a:'1 = (b:_ -> b:System.Int32):_):'1
            (rec a:'1 = (b:_ -> b:System.Int32):'1):'1
            (rec a:'1 = (b:'2 -> b:System.Int32):'1):'1      : Bind(b:'2)
            (rec a:'1 = (b:'2 -> b:System.Int32):'1):'1      : Lookup(b => '2), Memoize('2 => System.Int32)
            2:-------------------
            (rec a:'1 = (b:System.Int32 -> b:System.Int32):'1):'1      : Update('2 => System.Int32), Memoize('1 => (System.Int32 -> System.Int32))
            (rec a:'1 = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1      : Update('1 => (System.Int32 -> System.Int32))
            (rec a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1      : Update('1 => (System.Int32 -> System.Int32))
            (rec a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)      : Update('1 => (System.Int32 -> System.Int32))
            3:-------------------
            System.Int32 -> System.Int32
            */

            var term = RecursiveBind(Bound("a"), Lambda(Bound("b"), Free("b", Implicit("System.Int32"))));
            Assert.AreEqual("(rec a:_ = (b:_ -> b:System.Int32:_):_):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("(rec a:(System.Int32:'a -> System.Int32:'a):('a -> 'a) = (b:System.Int32:'a -> b:System.Int32:'a):(System.Int32:'a -> System.Int32:'a):('a -> 'a)):(System.Int32:'a -> System.Int32:'a):('a -> 'a)", inferred.AnnotatedReadableString);
        }

        [Test]
        public void RecursiveBind4()
        {
            var context = Terrain.Create();

            /*
            Recursive bind 4:
            rec a = b:System.Int32 -> b
            (rec a:_ = (b:System.Int32 -> b:_):_):_
            1:-------------------
            (rec a:_ = (b:System.Int32 -> b:_):_):'1
            (rec a:'1 = (b:System.Int32 -> b:_):_):'1
            (rec a:'1 = (b:System.Int32 -> b:_):'1):'1
            (rec a:'1 = (b:System.Int32 -> b:_):'1):'1                   : Bind(b:System.Int32)
            (rec a:'1 = (b:System.Int32 -> b:System.Int32):'1):'1        : Lookup(b => System.Int32), Memoize('1 => (System.Int32 -> System.Int32))
            2:-------------------
            (rec a:'1 = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1        : Update('1 => (System.Int32 -> System.Int32))
            (rec a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1        : Update('1 => (System.Int32 -> System.Int32))
            (rec a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)        : Update('1 => (System.Int32 -> System.Int32))
            3:-------------------
            System.Int32 -> System.Int32
            */

            var term = RecursiveBind(Bound("a"), Lambda(Bound("b", Implicit("System.Int32")), Free("b")));
            Assert.AreEqual("(rec a:_ = (b:System.Int32:_ -> b:_):_):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("(rec a:(System.Int32:'a -> System.Int32:'a):('a -> 'a) = (b:System.Int32:'a -> b:System.Int32:'a):(System.Int32:'a -> System.Int32:'a):('a -> 'a)):(System.Int32:'a -> System.Int32:'a):('a -> 'a)", inferred.AnnotatedReadableString);
        }

        [Test]
        public void RecursiveBind5()
        {
            var context = Terrain.Create();

            /*
            Recursive bind 5:
            rec a:System.Int32 = b -> b
            (rec a:System.Int32 = (b:_ -> b:_):_):_
            1:-------------------
            (rec a:System.Int32 = (b:_ -> b:_):_):'1
            (rec a:System.Int32 = (b:_ -> b:_):_):'1                        : Bind(a:System.Int32)
            (rec a:System.Int32 = (b:_ -> b:_):System.Int32):'1
            (rec a:System.Int32 = (b:'2 -> b:_):System.Int32):'1            : Bind(b:'2)
            (rec a:System.Int32 = (b:'2 -> b:'2):System.Int32):'1           : Lookup(b => '2)    // Unification problem (('2 -> '2) => System.Int32)
            */

            var term = RecursiveBind(Bound("a", Implicit("System.Int32")), Lambda(Bound("b"), Free("b")));
            Assert.AreEqual("(rec a:System.Int32:_ = (b:_ -> b:_):_):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.IsNull(inferred);
            Assert.AreEqual("Cannot unify: between \"('2:_ -> '2:_):(_ -> _)\" and \"System.Int32:_\"", errors.First().Details);
        }

        [Test]
        public void RecursiveBind6()
        {
            var context = Terrain.Create();

            /*
            Recursive bind 6:
            rec a:(System.Int32 -> _) = b -> b
            (rec a:(System.Int32 -> _) = (b:_ -> b:_):_):_
            1:-------------------
            (rec a:(System.Int32 -> _) = (b:_ -> b:_):_):'1
            (rec a:(System.Int32 -> '2) = (b:_ -> b:_):_):'1        : Memoize('1 => (System.Int32 -> '2))
            (rec a:(System.Int32 -> '2) = (b:_ -> b:_):(System.Int32 -> '2)):'1
            (rec a:(System.Int32 -> '2) = (b:System.Int32 -> b:_):(System.Int32 -> '2)):'1      : Bind(b:System.Int32)
            (rec a:(System.Int32 -> '2) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> '2)):'1     : Lookup(b => System.Int32), Memoize((System.Int32 -> '2) => (System.Int32 -> System.Int32))
            2:-------------------
            (rec a:(System.Int32 -> '2) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1     : Update((System.Int32 -> '2) => (System.Int32 -> System.Int32))
            (rec a:(System.Int32 -> '2) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> '2)       : Update('1 => (System.Int32 -> '2))
            (rec a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> '2)       : Update((System.Int32 -> '2) => (System.Int32 -> System.Int32))
            (rec a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)       : Update((System.Int32 -> '2) => (System.Int32 -> System.Int32))
            3:-------------------
            System.Int32 -> System.Int32
            */

            var term = RecursiveBind(Bound("a", Lambda(Bound("System.Int32"), Unspecified)), Lambda(Bound("b"), Free("b")));
            Assert.AreEqual("(rec a:(System.Int32:_ -> _):_ = (b:_ -> b:_):_):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("(rec a:(System.Int32:'a -> System.Int32:'a):('a -> 'a) = (b:System.Int32:'a -> b:System.Int32:'a):(System.Int32:'a -> System.Int32:'a)):(System.Int32:'a -> System.Int32:'a)", inferred.AnnotatedReadableString);
        }

        [Test]
        public void RecursiveBind7()
        {
            var context = Terrain.Create();

            /*
            rec a = b -> a
            (rec a:_ = (b:_ -> a:_):_):_
            1:-------------------
            (rec a:_ = (b:_ -> a:_):_):'1
            (rec a:'1 = (b:_ -> a:_):_):'1                  : Bind(a:'1)
            (rec a:'1 = (b:_ -> a:_):'1):'1
            (rec a:'1 = (b:'2 -> a:_):'1):'1                : Bind(b:'2)
            (rec a:'1 = (b:'2 -> a:'1):('2 -> '1)):'1       : Lookup(a => '1), Memoize('1 => ('2 -> '1))
            2:-------------------
            (rec a:'1 = (b:'2 -> a:'1):('2 -> '1)):'1       : Update('1 => ('2 -> '1))     // Recursive unification problem ('1 => ('2 -> '1))
            */

            var term = RecursiveBind(Bound("a"), Lambda(Bound("b"), Free("a")));
            Assert.AreEqual("(rec a:_ = (b:_ -> a:_):_):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("Recursive unification problem: '1:_ ... ('2:_ -> '1:_):_", errors.First().Details);
        }

        [Test]
        public void RecursiveBind8()
        {
            var context = Terrain.Create();

            /*
            rec a = rec b = a b
            (rec a:_ = (rec b:_ = (a:_ b:_):_):_):_
            1:-------------------
            (rec a:_ = (rec b:_ = (a:_ b:_):_):_):'1
            (rec a:'1 = (rec b:_ = (a:_ b:_):_):_):'1               : Bind(a:'1)
            (rec a:'1 = (rec b:_ = (a:_ b:_):_):'1):'1
            (rec a:'1 = (rec b:'1 = (a:_ b:_):_):'1):'1             : Bind(b:'1)
            (rec a:'1 = (rec b:'1 = (a:_ b:_):'1):'1):'1
            (rec a:'1 = (rec b:'1 = (a:_ b:'1):'1):'1):'1           : Lookup(b => '1)
            (rec a:'1 = (rec b:'1 = (a:('1 -> '1) b:'1):'1):'1):'1           : Memoize('1 => ('1 -> '1))
            2:-------------------
            (rec a:'1 = (rec b:'1 = (a:'1 b:'1):'1):'1):'1          : Update('1 => ('1 -> '1))     // Recursive unification problem ('1 => ('1 -> '1))
            */

            var term = RecursiveBind(Bound("a"), RecursiveBind(Bound("b"), Apply(Free("a"), Free("b"))));
            Assert.AreEqual("(rec a:_ = (rec b:_ = (a:_ b:_):_):_):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("Recursive unification problem: '1:_ ... '1:_ -> '1:_", errors.First().Details);
        }

        [Test]
        public void RecursiveBind9()
        {
            var context = Terrain.Create();

            /*
            rec a = a b
            (rec a:_ = (a:_ b:_):_):_
            1:-------------------
            (rec a:_ = (a:_ b:_):_):'1
            (rec a:'1 = (a:_ b:_):_):'1
            (rec a:'1 = (a:_ b:_):'1):'1
            (rec a:'1 = (a:_ b:'2):'1):'1
            (rec a:'1 = (a:('2 -> '1) b:'2):'1):'1                   : Memoize('1 => ('2 -> '1))
            2:-------------------
            (rec a:'1 = (a:('2 -> '1) b:'2):('2 -> '1)):'1           : Update('1 => ('2 -> '1))     // Recursive unification problem ('1 => ('2 -> '1))
            3:-------------------
            '2 -> '1
            */

            var term = RecursiveBind(Bound("a"), Apply(Free("a"), Implicit("b")));
            Assert.AreEqual("(rec a:_ = (a:_ b:_):_):_", term.StrictReadableString);

            var (inferred, errors) = context.Infer(term, Term.Unspecified);
            Assert.AreEqual("Recursive unification problem: '1:_ ... '2:_ -> '1:_", errors.First().Details);
        }
    }
}
