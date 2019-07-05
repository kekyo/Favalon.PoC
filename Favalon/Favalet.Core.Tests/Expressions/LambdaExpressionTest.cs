using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    using static Internals.StaticFactories;

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

            var expression = Lambda(Variable("a"), Variable("a"));
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

            var expression = Lambda(Variable("a"), Variable("a", Variable("System.Int32")));
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

            var expression = Lambda(Variable("a", Variable("System.Int32")), Variable("a"));
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

            var expression = Lambda(Variable("a"), Lambda(Variable("b"), Variable("a")));
            Assert.AreEqual("(a:? -> (b:? -> a:?):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:'2 -> (b:'4 -> a:'2):('4 -> '2)):('2 -> ('4 -> '2))", inferred.StrictReadableString);
        }
    }
}