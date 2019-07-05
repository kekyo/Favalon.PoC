using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    using static StaticFactories;

    [TestFixture]
    public sealed class BindExpressionTest
    {
        [Test]
        public void Bind1()
        {
            var environment = Environment.Create();

            /*
            Bind 1:
            a = 123
            (a:? = 123:?):?
            1:-------------------
            (a:? = 123:?):'1
            (a:? = 123:Numeric):'1                       : Memoize('1 => Numeric)
            (a:Numeric = 123:Numeric):'1
            2:-------------------
            (a:Numeric = 123:Numeric):Numeric            : Update('1 => Numeric)
            3:-------------------
            Numeric
            */

            var expression = Bind(Bound("a"), Literal(123));
            Assert.AreEqual("(a:? = 123:?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:Numeric = 123:Numeric):Numeric", inferred.StrictReadableString);
        }

        [Test]
        public void Bind2()
        {
            var environment = Environment.Create();

            /*
            Bind 2:
            a = b -> b
            (a:? = (b:? -> b:?):?):?
            1:-------------------
            (a:? = (b:? -> b:?):?):'1
            (a:? = (b:? -> b:?):'1):'1
            (a:? = (b:'2 -> b:?):'1):'1                  : Bind(b:'2)
            (a:? = (b:'2 -> b:'2):'1):'1                 : Lookup(b => '2), Memoize('1 => ('2 -> '2))
            (a:'1 = (b:'2 -> b:'2):'1):'1
            2:-------------------
            (a:'1 = (b:'2 -> b:'2):('2 -> '2)):'1                        : Update('1 => ('2 -> '2))
            (a:('2 -> '2) = (b:'2 -> b:'2):('2 -> '2)):'1                : Update('1 => ('2 -> '2))
            (a:('2 -> '2) = (b:'2 -> b:'2):('2 -> '2)):('2 -> '2)        : Update('1 => ('2 -> '2))
            3:-------------------
            '2 -> '2
            */

            var expression = Bind(Bound("a"), Lambda(Bound("b"), Free("b")));
            Assert.AreEqual("(a:? = (b:? -> b:?):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:('2 -> '2) = (b:'2 -> b:'2):('2 -> '2)):('2 -> '2)", inferred.StrictReadableString);
        }

        [Test]
        public void Bind3()
        {
            var environment = Environment.Create();

            /*
            Bind 3:
            a = b -> b:System.Int32
            (a:? = (b:? -> b:System.Int32):?):?
            1:-------------------
            (a:? = (b:? -> b:System.Int32):?):'1
            (a:'1 = (b:? -> b:System.Int32):?):'1
            (a:'1 = (b:? -> b:System.Int32):'1):'1
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

            var expression = Bind(Bound("a"), Lambda(Bound("b"), Free("b", Implicit("System.Int32"))));
            Assert.AreEqual("(a:? = (b:? -> b:System.Int32):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)", inferred.StrictReadableString);
        }
    }
}
