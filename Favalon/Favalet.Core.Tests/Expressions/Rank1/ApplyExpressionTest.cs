using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Rank1
{
    using static StaticFactories;

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

            var expression = Apply(Implicit("a"), Implicit("b"));
            Assert.AreEqual("(a:? b:?):?", expression.StrictReadableString);

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

            var expression = Apply(Implicit("a"), Implicit("b", Implicit("System.Int32")));
            Assert.AreEqual("(a:? b:System.Int32):?", expression.StrictReadableString);

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

            var expression = Apply(Implicit("a", Lambda(Bound("System.Int32"), Unspecified)), Implicit("b"));
            Assert.AreEqual("(a:(System.Int32 -> ?) b:?):?", expression.StrictReadableString);

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

            var expression = Apply(Apply(Implicit("a"), Implicit("b")), Implicit("c"));
            Assert.AreEqual("((a:? b:?):? c:?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("((a:('3 -> ('2 -> '1)) b:'3):('2 -> '1) c:'2):'1", inferred.StrictReadableString);
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

            var expression = Apply(Apply(Implicit("a"), Implicit("b")), Implicit("c", Implicit("System.Int32")));
            Assert.AreEqual("((a:? b:?):? c:System.Int32):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("((a:('2 -> (System.Int32 -> '1)) b:'2):(System.Int32 -> '1) c:System.Int32):'1", inferred.StrictReadableString);
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

            var expression = Apply(Apply(Implicit("a"), Implicit("b", Implicit("System.Int32"))), Implicit("c"));
            Assert.AreEqual("((a:? b:System.Int32):? c:?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("((a:(System.Int32 -> ('2 -> '1)) b:System.Int32):('2 -> '1) c:'2):'1", inferred.StrictReadableString);
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

            var expression = Apply(Apply(Implicit("a", Lambda(Bound("System.Int32"), Unspecified)), Implicit("b")), Implicit("c"));
            Assert.AreEqual("((a:(System.Int32 -> ?) b:?):? c:?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("((a:(System.Int32 -> ('2 -> '1)) b:System.Int32):('2 -> '1) c:'2):'1", inferred.StrictReadableString);
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

            var expression = Apply(Implicit("a"), Apply(Implicit("b"), Implicit("c")));
            Assert.AreEqual("(a:? (b:? c:?):?):?", expression.StrictReadableString);

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
            (a:('1 -> System.Int32) (b:('2 -> '1) c:'2):'1):System.Int32
            2:-------------------
            3:-------------------
            System.Int32
            */

            var expression = Apply(Implicit("a"), Apply(Implicit("b"), Implicit("c")), Implicit("System.Int32"));
            Assert.AreEqual("(a:? (b:? c:?):?):System.Int32", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:('1 -> System.Int32) (b:('2 -> '1) c:'2):'1):System.Int32", inferred.StrictReadableString);
        }

        [Test]
        public void Apply10()
        {
            var environment = Environment.Create();

            /*
            Apply 10:
            a (b c):System.Int32
            (a:? (b:? c:?):System.Int32):?
            1:-------------------
            (a:? (b:? c:?):System.Int32):'1
            (a:? (b:? c:'2):System.Int32):'1
            (a:? (b:('2 -> System.Int32) c:'2):System.Int32):'1
            (a:(System.Int32 -> '1) (b:('2 -> System.Int32) c:'2):System.Int32):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Implicit("a"), Apply(Implicit("b"), Implicit("c"), Implicit("System.Int32")));
            Assert.AreEqual("(a:? (b:? c:?):System.Int32):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:(System.Int32 -> '1) (b:('2 -> System.Int32) c:'2):System.Int32):'1", inferred.StrictReadableString);
        }

        [Test]
        public void Apply11()
        {
            var environment = Environment.Create();

            /*
            Apply 11:
            a (b c:System.Int32)
            (a:? (b:? c:System.Int32):?):?
            1:-------------------
            (a:? (b:? c:System.Int32):?):'1
            (a:? (b:? c:System.Int32):'2):'1
            (a:? (b:(System.Int32 -> '2) c:System.Int32):'2):'1
            (a:('2 -> '1) (b:(System.Int32 -> '2) c:System.Int32):'2):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Implicit("a"), Apply(Implicit("b"), Implicit("c", Implicit("System.Int32"))));
            Assert.AreEqual("(a:? (b:? c:System.Int32):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:('2 -> '1) (b:(System.Int32 -> '2) c:System.Int32):'2):'1", inferred.StrictReadableString);
        }

        [Test]
        public void Apply12()
        {
            var environment = Environment.Create();

            /*
            Apply 12:
            a (b:(System.Int32 -> ?) c)
            (a:? (b:(System.Int32 -> ?) c:?):?):?
            1:-------------------
            (a:? (b:(System.Int32 -> ?) c:?):?):'1
            (a:? (b:(System.Int32 -> ?) c:?):'2):'1
            (a:? (b:(System.Int32 -> ?) c:'3):'2):'1
            (a:? (b:(System.Int32 -> '2) c:'3):'2):'1                           : Memoize('3 => System.Int32)
            (a:('2 -> '1) (b:(System.Int32 -> '2) c:'3):'2):'1
            2:-------------------
            (a:('2 -> '1) (b:(System.Int32 -> '2) c:System.Int32):'2):'1                 : Update('3 => System.Int32)
            3:-------------------
            '1
            */

            var expression = Apply(Implicit("a"), Apply(Implicit("b", Lambda(Bound("System.Int32"), Unspecified)), Implicit("c")));
            Assert.AreEqual("(a:? (b:(System.Int32 -> ?) c:?):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:('2 -> '1) (b:(System.Int32 -> '2) c:System.Int32):'2):'1", inferred.StrictReadableString);
        }

        [Test]
        public void Apply13()
        {
            var environment = Environment.Create();

            /*
            Apply 13:
            a:(System.Int32 -> ? -> ?) (b c)
            (a:((System.Int32 -> ?) -> ?) (b:? c:?):?):?
            1:-------------------
            (a:((System.Int32 -> ?) -> ?) (b:? c:?):?):'1
            (a:((System.Int32 -> ?) -> ?) (b:? c:?):'2):'1
            (a:((System.Int32 -> ?) -> ?) (b:? c:'3):'2):'1
            (a:((System.Int32 -> ?) -> ?) (b:('3 -> '2) c:'3):'2):'1
            (a:((System.Int32 -> '4) -> '1) (b:('3 -> '2) c:'3):'2):'1                       : Memoize('2 => (System.Int32 -> '4))
            2:-------------------
            (a:((System.Int32 -> '4) -> '1) (b:('3 -> '2) c:'3):(System.Int32 -> '4)):'1     : Update('2 => (System.Int32 -> '4))
            (a:((System.Int32 -> '4) -> '1) (b:('3 -> (System.Int32 -> '4)) c:'3):(System.Int32 -> '4)):'1     : Update('2 => (System.Int32 -> '4))
            */

            var expression = Apply(Implicit("a", Lambda(Lambda(Bound("System.Int32"), Unspecified), Unspecified)), Apply(Implicit("b"), Implicit("c")));
            Assert.AreEqual("(a:((System.Int32 -> ?) -> ?) (b:? c:?):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:((System.Int32 -> '4) -> '1) (b:('3 -> (System.Int32 -> '4)) c:'3):(System.Int32 -> '4)):'1", inferred.StrictReadableString);
        }
    }
}