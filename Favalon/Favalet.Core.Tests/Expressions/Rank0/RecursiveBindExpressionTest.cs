using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Rank0
{
    using static StaticFactories;

    [TestFixture]
    public sealed class RecursiveBindExpressionTest
    {
        [Test]
        public void RecursiveBind1()
        {
            var environment = Environment.Create();

            /*
            Recursive bind 1:
            rec a = 123
            (rec a:? = 123:?):?
            1:-------------------
            (rec a:? = 123:?):'1
            (rec a:'1 = 123:Numeric):'1                      : Bind(a:'1)
            (rec a:'1 = 123:Numeric):'1                      : Memoize('1 => Numeric)
            2:-------------------
            (rec a:Numeric = 123:Numeric):'1                 : Update('1 => Numeric)
            (rec a:Numeric = 123:Numeric):Numeric            : Update('1 => Numeric)
            3:-------------------
            Numeric
            */

            var expression = RecursiveBind(Bound("a"), Literal(123));
            Assert.AreEqual("(rec a:? = 123:?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(rec a:Numeric = 123:Numeric):Numeric", inferred.StrictReadableString);
        }

        [Test]
        public void RecursiveBind2()
        {
            var environment = Environment.Create();

            /*
            Recursive bind 2:
            rec a = b -> b
            (rec a:? = (b:? -> b:?):?):?
            1:-------------------
            (rec a:? = (b:? -> b:?):?):'1
            (rec a:? = (b:? -> b:?):'1):'1
            (rec a:? = (b:'2 -> b:?):'1):'1                  : Bind(b:'2)
            (rec a:? = (b:'2 -> b:'2):'1):'1                 : Lookup(b => '2), Memoize('1 => ('2 -> '2))
            (rec a:'1 = (b:'2 -> b:'2):'1):'1
            2:-------------------
            (rec a:'1 = (b:'2 -> b:'2):('2 -> '2)):'1                        : Update('1 => ('2 -> '2))
            (rec a:('2 -> '2) = (b:'2 -> b:'2):('2 -> '2)):'1                : Update('1 => ('2 -> '2))
            (rec a:('2 -> '2) = (b:'2 -> b:'2):('2 -> '2)):('2 -> '2)        : Update('1 => ('2 -> '2))
            3:-------------------
            '2 -> '2
            */

            var expression = RecursiveBind(Bound("a"), Lambda(Bound("b"), Free("b")));
            Assert.AreEqual("(rec a:? = (b:? -> b:?):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(rec a:('2 -> '2) = (b:'2 -> b:'2):('2 -> '2)):('2 -> '2)", inferred.StrictReadableString);
        }

        [Test]
        public void RecursiveBind3()
        {
            var environment = Environment.Create();

            /*
            Recursive bind 3:
            rec a = b -> b:System.Int32
            (rec a:? = (b:? -> b:System.Int32):?):?
            1:-------------------
            (rec a:? = (b:? -> b:System.Int32):?):'1
            (rec a:'1 = (b:? -> b:System.Int32):?):'1
            (rec a:'1 = (b:? -> b:System.Int32):'1):'1
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

            var expression = RecursiveBind(Bound("a"), Lambda(Bound("b"), Free("b", Implicit("System.Int32"))));
            Assert.AreEqual("(rec a:? = (b:? -> b:System.Int32):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(rec a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)", inferred.StrictReadableString);
        }

        [Test]
        public void RecursiveBind4()
        {
            var environment = Environment.Create();

            /*
            Recursive bind 4:
            rec a = b:System.Int32 -> b
            (rec a:? = (b:System.Int32 -> b:?):?):?
            1:-------------------
            (rec a:? = (b:System.Int32 -> b:?):?):'1
            (rec a:'1 = (b:System.Int32 -> b:?):?):'1
            (rec a:'1 = (b:System.Int32 -> b:?):'1):'1
            (rec a:'1 = (b:System.Int32 -> b:?):'1):'1                   : Bind(b:System.Int32)
            (rec a:'1 = (b:System.Int32 -> b:System.Int32):'1):'1        : Lookup(b => System.Int32), Memoize('1 => (System.Int32 -> System.Int32))
            2:-------------------
            (rec a:'1 = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1        : Update('1 => (System.Int32 -> System.Int32))
            (rec a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1        : Update('1 => (System.Int32 -> System.Int32))
            (rec a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)        : Update('1 => (System.Int32 -> System.Int32))
            3:-------------------
            System.Int32 -> System.Int32
            */

            var expression = RecursiveBind(Bound("a"), Lambda(Bound("b", Implicit("System.Int32")), Free("b")));
            Assert.AreEqual("(rec a:? = (b:System.Int32 -> b:?):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(rec a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)", inferred.StrictReadableString);
        }

        [Test]
        public void RecursiveBind5()
        {
            var environment = Environment.Create();

            /*
            Recursive bind 5:
            rec a:System.Int32 = b -> b
            (rec a:System.Int32 = (b:? -> b:?):?):?
            1:-------------------
            (rec a:System.Int32 = (b:? -> b:?):?):'1
            (rec a:System.Int32 = (b:? -> b:?):'1):'1
            (rec a:System.Int32 = (b:'2 -> b:?):'1):'1                      : Bind(b:'2)
            (rec a:System.Int32 = (b:'2 -> b:'2):('2 -> '2)):'1             : Lookup(b => '2)
            (rec a:System.Int32 = (b:'2 -> b:'2):('2 -> '2)):'1
            (rec a:System.Int32 = (b:'2 -> b:'2):('2 -> '2)):'1             : Unification problem (('2 -> '2) => System.Int32)
            */

            var expression = RecursiveBind(Bound("a", Implicit("System.Int32")), Lambda(Bound("b"), Free("b")));
            Assert.AreEqual("(rec a:System.Int32 = (b:? -> b:?):?):?", expression.StrictReadableString);

            Assert.Throws<ArgumentException>(() => environment.Infer(expression));
        }

        [Test]
        public void RecursiveBind6()
        {
            var environment = Environment.Create();

            /*
            Recursive bind 6:
            rec a:(System.Int32 -> ?) = b -> b
            (rec a:(System.Int32 -> ?) = (b:? -> b:?):?):?
            1:-------------------
            (rec a:(System.Int32 -> ?) = (b:? -> b:?):?):'1
            (rec a:(System.Int32 -> '2) = (b:? -> b:?):?):'1        : Memoize('1 => (System.Int32 -> '2))
            (rec a:(System.Int32 -> '2) = (b:? -> b:?):(System.Int32 -> '2)):'1
            (rec a:(System.Int32 -> '2) = (b:System.Int32 -> b:?):(System.Int32 -> '2)):'1      : Bind(b:System.Int32)
            (rec a:(System.Int32 -> '2) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> '2)):'1     : Lookup(b => System.Int32), Memoize((System.Int32 -> '2) => (System.Int32 -> System.Int32))
            2:-------------------
            (rec a:(System.Int32 -> '2) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):'1     : Update((System.Int32 -> '2) => (System.Int32 -> System.Int32))
            (rec a:(System.Int32 -> '2) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> '2)       : Update('1 => (System.Int32 -> '2))
            (rec a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> '2)       : Update((System.Int32 -> '2) => (System.Int32 -> System.Int32))
            (rec a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)       : Update((System.Int32 -> '2) => (System.Int32 -> System.Int32))
            3:-------------------
            System.Int32 -> System.Int32
            */

            var expression = RecursiveBind(Bound("a", Lambda(Bound("System.Int32"), Unspecified)), Lambda(Bound("b"), Free("b")));
            Assert.AreEqual("(rec a:(System.Int32 -> ?) = (b:? -> b:?):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(rec a:(System.Int32 -> System.Int32) = (b:System.Int32 -> b:System.Int32):(System.Int32 -> System.Int32)):(System.Int32 -> System.Int32)", inferred.StrictReadableString);
        }

        [Test]
        public void RecursiveBind7()
        {
            var environment = Environment.Create();

            /*
            rec a = b -> a
            (rec a:? = (b:? -> a:?):?):?
            1:-------------------
            (rec a:? = (b:? -> a:?):?):'1
            (rec a:'1 = (b:? -> a:?):?):'1                  : RecBind(a:'1)
            (rec a:'1 = (b:? -> a:?):'1):'1
            (rec a:'1 = (b:'2 -> a:?):'1):'1                : Bind(b:'2)
            (rec a:'1 = (b:'2 -> a:'1):('2 -> '1)):'1               : Lookup(a => '1), Memoize('1 => ('2 -> '1))
            2:-------------------
            (rec a:'1 = (b:'2 -> a:'1):('2 -> '1)):'1       : Update('1 => ('2 -> '1))     // Recursive unification problem ('1 => ('2 -> '1))
            */

            var expression = RecursiveBind(Bound("a"), Lambda(Bound("b"), Free("a")));
            Assert.AreEqual("(rec a:? = (b:? -> a:?):?):?", expression.StrictReadableString);

            Assert.Throws<ArgumentException>(() => environment.Infer(expression));
        }

        [Test]
        public void RecursiveBind8()
        {
            var environment = Environment.Create();

            /*
            rec a = rec b = a b
            (rec a:? = (rec b:? = (a:? b:?):?):?):?
            1:-------------------
            (rec a:? = (rec b:? = (a:? b:?):?):?):'1
            (rec a:'1 = (rec b:? = (a:? b:?):?):?):'1               : RecBind(a:'1)
            (rec a:'1 = (rec b:? = (a:? b:?):?):'1):'1
            (rec a:'1 = (rec b:'1 = (a:? b:?):?):'1):'1             : RecBind(b:'1)
            (rec a:'1 = (rec b:'1 = (a:? b:?):'1):'1):'1
            (rec a:'1 = (rec b:'1 = (a:? b:'1):'1):'1):'1           : Lookup(b => '1)
            (rec a:'1 = (rec b:'1 = (a:('1 -> '1) b:'1):'1):'1):'1           : Memoize('1 => ('1 -> '1))
            2:-------------------
            (rec a:'1 = (rec b:'1 = (a:'1 b:'1):'1):'1):'1          : Update('1 => ('1 -> '1))     // Recursive unification problem ('1 => ('1 -> '1))
            */

            var expression = RecursiveBind(Bound("a"), RecursiveBind(Bound("b"), Apply(Free("a"), Free("b"))));
            Assert.AreEqual("(rec a:? = (rec b:? = (a:? b:?):?):?):?", expression.StrictReadableString);

            Assert.Throws<ArgumentException>(() => environment.Infer(expression));
        }

        [Test]
        public void RecursiveBind9()
        {
            var environment = Environment.Create();

            /*
            rec a = a b
            (rec a:? = (a:? b:?):?):?
            1:-------------------
            (rec a:? = (a:? b:?):?):'1
            (rec a:'1 = (a:? b:?):?):'1
            (rec a:'1 = (a:? b:?):'1):'1
            (rec a:'1 = (a:? b:'2):'1):'1
            (rec a:'1 = (a:('2 -> '1) b:'2):'1):'1                   : Memoize('1 => ('2 -> '1))
            2:-------------------
            (rec a:'1 = (a:('2 -> '1) b:'2):('2 -> '1)):'1           : Update('1 => ('2 -> '1))     // Recursive unification problem ('1 => ('1 -> '1))
            3:-------------------
            '2 -> '1
            */

            var expression = RecursiveBind(Bound("a"), Apply(Free("a"), Implicit("b")));
            Assert.AreEqual("(rec a:? = (a:? b:?):?):?", expression.StrictReadableString);

            Assert.Throws<ArgumentException>(() => environment.Infer(expression));
        }
    }
}
