using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    using static Internals.StaticFactories;

    [TestFixture]
    public sealed class ApplyExpressionTest
    {
        [Test]
        public void Apply1()
        {
            var environment = Environment.Create();

            /*
            Apply 1:
            a b
            (a:? b:?):?
            1:-------------------
            (a:? b:?):'1
            (a:? b:'2):'1
            (a:('2 -> '1) b:'2):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Variable("a"), Variable("b"));

            var inferred = environment.Infer(expression);

            Assert.AreEqual("(a:('2 -> '1) b:'2):'1", inferred.StrictReadableString);
        }

        [Test]
        public void Apply2()
        {
            var environment = Environment.Create();

            /*
            Apply 2:
            a b:System.Int32
            (a:? b:System.Int32):?
            1:-------------------
            (a:? b:System.Int32):'1
            (a:(System.Int32 -> '1) b:System.Int32):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Variable("a"), Variable("b", Variable("System.Int32")));

            var inferred = environment.Infer(expression);

            Assert.AreEqual("(a:(System.Int32 -> '1) b:System.Int32):'1", inferred.StrictReadableString);
        }

        [Test]
        public void Apply3()
        {
            var environment = Environment.Create();

            /*
            Apply 3:
            a:(System.Int32 -> ?) b
            (a:(System.Int32 -> ?) b:?):?
            1:-------------------
            (a:(System.Int32 -> ?) b:?):'1               : Hint('1)
            (a:(System.Int32 -> ?) b:'2):'1              : Hint('2)
            (a:(System.Int32 -> '1) b:'2):'1             : Hint('2 -> '1), Memoize('2 => System.Int32)
            2:-------------------
            (a:(System.Int32 -> '1) b:System.Int32):'1   : Update('2 => System.Int32)
            3:-------------------
            '1
            */

            var expression = Apply(Variable("a", Lambda(Variable("System.Int32"), Undefined())), Variable("b"));

            var inferred = environment.Infer(expression);

            Assert.AreEqual("(a:(System.Int32 -> '1) b:System.Int32):'1", inferred.StrictReadableString);
        }

        [Test]
        public void Apply4()
        {
            var environment = Environment.Create();

            /*
            Apply 4:
            a b c
            ((a:? b:?):? c:?):?
            1:-------------------
            ((a:? b:?):? c:?):'1
            ((a:? b:?):? c:'2):'1
            ((a:? b:?):('2 -> '1) c:'2):'1
            ((a:? b:'3):('2 -> '1) c:'2):'1
            ((a:('3 -> ('2 -> '1)) b:'3):('2 -> '1) c:'2):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Apply(Variable("a"), Variable("b")), Variable("c"));

            var inferred = environment.Infer(expression);

            Assert.AreEqual("((a:('3 -> '2 -> '1) b:'3):('2 -> '1) c:'2):'1", inferred.StrictReadableString);
        }

        [Test]
        public void Apply5()
        {
            var environment = Environment.Create();

            /*
            Apply 5:
            a b c:System.Int32
            ((a:? b:?):? c:System.Int32):?
            1:-------------------
            ((a:? b:?):? c:System.Int32):'1
            ((a:? b:?):(System.Int32 -> '1) c:System.Int32):'1
            ((a:? b:'2):(System.Int32 -> '1) c:System.Int32):'1
            ((a:('2 -> (System.Int32 -> '1)) b:'2):(System.Int32 -> '1) c:System.Int32):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Apply(Variable("a"), Variable("b")), Variable("c", Variable("System.Int32")));

            var inferred = environment.Infer(expression);

            Assert.AreEqual("((a:('2 -> System.Int32 -> '1) b:'2):(System.Int32 -> '1) c:System.Int32):'1", inferred.StrictReadableString);
        }

        [Test]
        public void Apply6()
        {
            var environment = Environment.Create();

            /*
            Apply 6:
            a b:System.Int32 c
            ((a:? b:System.Int32):? c:?):?
            1:-------------------
            ((a:? b:System.Int32):? c:?):'1
            ((a:? b:System.Int32):? c:'2):'1
            ((a:? b:System.Int32):('2 -> '1) c:'2):'1
            ((a:(System.Int32 -> ('2 -> '1)) b:System.Int32):('2 -> '1) c:'2):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Apply(Variable("a"), Variable("b", Variable("System.Int32"))), Variable("c"));

            var inferred = environment.Infer(expression);

            Assert.AreEqual("((a:(System.Int32 -> '2 -> '1) b:System.Int32):('2 -> '1) c:'2):'1", inferred.StrictReadableString);
        }

        [Test]
        public void Apply7()
        {
            var environment = Environment.Create();

            /*
            Apply 7:
            a:(System.Int32 -> ?) b c
            ((a:(System.Int32 -> ?) b:?):? c:?):?
            1:-------------------
            ((a:(System.Int32 -> ?) b:?):? c:?):'1                                 : Hint('1)
            ((a:(System.Int32 -> ?) b:?):? c:'2):'1                                : Hint('2)
            ((a:(System.Int32 -> ?) b:?):('2 -> '1) c:'2):'1                       : Hint(('2 -> '1))
            ((a:(System.Int32 -> ?) b:'3):('2 -> '1) c:'2):'1                      : Hint('3)
            ((a:(System.Int32 -> ('2 -> '1)) b:'3):('2 -> '1) c:'2):'1             : Hint('3 -> ('2 -> '1)), Memoize('3 => System.Int32)
            2:-------------------
            ((a:(System.Int32 -> ('2 -> '1)) b:System.Int32):('2 -> '1) c:'2):'1   : Update('3 => System.Int32)
            3:-------------------
            '1
            */

            var expression = Apply(Apply(Variable("a", Lambda(Variable("System.Int32"), Undefined())), Variable("b")), Variable("c"));

            var inferred = environment.Infer(expression);

            Assert.AreEqual("((a:(System.Int32 -> '2 -> '1) b:System.Int32):('2 -> '1) c:'2):'1", inferred.StrictReadableString);
        }

        [Test]
        public void Apply8()
        {
            var environment = Environment.Create();

            /*
            Apply 8:
            a (b c)
            (a:? (b:? c:?):?):?
            1:-------------------
            (a:? (b:? c:?):?):'1
            (a:? (b:? c:?):'2):'1
            (a:('2 -> '1) (b:? c:?):'2):'1
            (a:('2 -> '1) (b:? c:'3):'2):'1
            (a:('2 -> '1) (b:('3 -> '2) c:'3):'2):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Variable("a"), Apply(Variable("b"), Variable("c")));

            var inferred = environment.Infer(expression);

            Assert.AreEqual("(a:('2 -> '1) (b:('3 -> '2) c:'3):'2):'1", inferred.StrictReadableString);
        }

        [Test]
        public void Apply9()
        {
            var environment = Environment.Create();

            /*
            Apply 9:
            (a (b c)):System.Int32
            (a:? (b:? c:?):?):System.Int32
            1:-------------------
            (a:? (b:? c:?):'1):System.Int32
            (a:? (b:? c:?):'1):System.Int32
            (a:? (b:? c:'2):'1):System.Int32
            (a:? (b:('2 -> '1) c:'2):'1):System.Int32
            2:-------------------
            (a:('1 -> System.Int32) (b:('2 -> '1) c:'2):'1):System.Int32
            3:-------------------
            System.Int32
            */

            var expression = Apply(Variable("a"), Apply(Variable("b"), Variable("c")), Variable("System.Int32"));

            var inferred = environment.Infer(expression);

            Assert.AreEqual("(a:('1 -> System.Int32) (b:('2 -> '1) c:'2):'1):System.Int32", inferred.StrictReadableString);
        }
    }
}
