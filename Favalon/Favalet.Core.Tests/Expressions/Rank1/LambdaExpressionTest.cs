﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Rank1
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
            (a:? -> a:?):?
            1:-------------------
            (a:? -> a:?):'1
            (a:'2 -> a:?):'1                     : Bind(a:'2)
            (a:'2 -> a:'2):'1                    : Lookup(a => '2), Memoize('1 => ('2 -> '2))
            2:-------------------
            (a:'2 -> a:'2):('2 -> '2)            : Update('1 => ('2 -> '2))
            3:-------------------
            '2 -> '2
            */

            var expression = Lambda(Bound("a"), Free("a"));
            Assert.AreEqual("(a:? -> a:?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:'2 -> a:'2):('2 -> '2)", inferred.StrictReadableString);
        }

        [Test]
        public void Lambda2()
        {
            var environment = Environment.Create();

            /*
            Lambda 2:
            a -> a:System.Int32
            (a:? -> a:System.Int32):?
            1:-------------------
            (a:? -> a:System.Int32):'1
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
            Assert.AreEqual("(a:? -> a:System.Int32):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:System.Int32 -> a:System.Int32):(System.Int32 -> System.Int32)", inferred.StrictReadableString);
        }

        [Test]
        public void Lambda3()
        {
            var environment = Environment.Create();

            /*
            Lambda 3:
            a:System.Int32 -> a
            (a:System.Int32 -> a:?):?
            1:-------------------
            (a:System.Int32 -> a:?):'1
            (a:System.Int32 -> a:?):'1                      : Bind(a:System.Int32)
            (a:System.Int32 -> a:System.Int32):'1           : Lookup(a => System.Int32), Memoize('1 => (System.Int32 -> System.Int32))
            2:-------------------
            (a:System.Int32 -> a:System.Int32):(System.Int32 -> System.Int32)           : Update('1 => (System.Int32 -> System.Int32))
            3:-------------------
            System.Int32 -> System.Int32
            */

            var expression = Lambda(Bound("a", Free("System.Int32")), Free("a"));
            Assert.AreEqual("(a:System.Int32 -> a:?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:System.Int32 -> a:System.Int32):(System.Int32 -> System.Int32)", inferred.StrictReadableString);
        }

        [Test]
        public void Lambda4()
        {
            var environment = Environment.Create();

            /*
            Lambda 4:
            a -> b -> a
            (a:? -> (b:? -> a:?):?):?
            1:-------------------
            (a:? -> (b:? -> a:?):?):'1
            (a:'2 -> (b:? -> a:?):?):'1                     : Bind(a:'2)
            (a:'2 -> (b:? -> a:?):'3):'1                    : Memoize('1 => ('2 -> '3))
            (a:'2 -> (b:'4 -> a:?):'3):'1                   : Bind(b:'4)
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
            Assert.AreEqual("(a:? -> (b:? -> a:?):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:'2 -> (b:'4 -> a:'2):('4 -> '2)):('2 -> ('4 -> '2))", inferred.StrictReadableString);
        }

        [Test]
        public void Lambda5()
        {
            var environment = Environment.Create();

            /*
            Lambda 5:
            a -> b -> b
            (a:? -> (b:? -> b:?):?):?
            1:-------------------
            (a:? -> (b:? -> b:?):?):'1
            (a:'2 -> (b:? -> b:?):?):'1                     : Bind(a:'2)
            (a:'2 -> (b:? -> b:?):'3):'1                    : Memoized('1 => ('2 -> '3))
            (a:'2 -> (b:'4 -> b:?):'3):'1                   : Bind(b:'4)
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
            Assert.AreEqual("(a:? -> (b:? -> b:?):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:'2 -> (b:'4 -> b:'4):('4 -> '4)):('2 -> ('4 -> '4))", inferred.StrictReadableString);
        }

        [Test]
        public void Lambda6()
        {
            var environment = Environment.Create();

            /*
            Lambda 6:
            a -> b -> a:System.Int32
            (a:? -> (b:? -> a:System.Int32):?):?
            1:-------------------
            (a:? -> (b:? -> a:System.Int32):?):'1
            (a:'2 -> (b:? -> a:System.Int32):?):'1          : Bind(a:'2)
            (a:'2 -> (b:? -> a:System.Int32):'3):'1         : Memoized('1 => ('2 -> '3))
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
            Assert.AreEqual("(a:? -> (b:? -> a:System.Int32):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:System.Int32 -> (b:'4 -> a:System.Int32):('4 -> System.Int32)):(System.Int32 -> ('4 -> System.Int32))", inferred.StrictReadableString);
        }

        [Test]
        public void Lambda7()
        {
            var environment = Environment.Create();

            /*
            Lambda 7:
            a -> b:System.Int32 -> a
            (a:? -> (b:System.Int32 -> a:?):?):?
            1:-------------------
            (a:? -> (b:System.Int32 -> a:?):?):'1
            (a:'2 -> (b:System.Int32 -> a:?):?):'1           : Bind(a:'2)
            (a:'2 -> (b:System.Int32 -> a:?):'3):'1          : Memoize('1 => ('2 -> '3))
            (a:'2 -> (b:System.Int32 -> a:?):'3):'1          : Bind(b:System.Int32)
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
            Assert.AreEqual("(a:? -> (b:System.Int32 -> a:?):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:'2 -> (b:System.Int32 -> a:'2):(System.Int32 -> '2)):('2 -> (System.Int32 -> '2))", inferred.StrictReadableString);
        }

        [Test]
        public void Lambda8()
        {
            var environment = Environment.Create();

            /*
            Lambda 8:
            a:System.Int32 -> b -> a
            (a:System.Int32 -> (b:? -> a:?):?):?
            1:-------------------
            (a:System.Int32 -> (b:? -> a:?):?):'1
            (a:System.Int32 -> (b:? -> a:?):?):'1             : Bind(a:System.Int32)
            (a:System.Int32 -> (b:? -> a:?):'2):'1            : Memoize('1 => (System.Int32 -> '2))
            (a:System.Int32 -> (b:'3 -> a:?):'2):'1           : Bind(b:'3)
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
            Assert.AreEqual("(a:System.Int32 -> (b:? -> a:?):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:System.Int32 -> (b:'3 -> a:System.Int32):('3 -> System.Int32)):(System.Int32 -> ('3 -> System.Int32))", inferred.StrictReadableString);
        }
    }
}