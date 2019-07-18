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
    public sealed class ApplyExpressionTest
    {
        [Test]
        public void Apply1_1()
        {
            var context = Terrain.Create();

            /*
            Apply 1:
            a:? b:?
            (a:? b:?):_
            1:-------------------
            (a:? b:?):'1:_
            (a:? b:'2:*):'1:_
            (a:('2:* -> '1:_) b:'2:*):'1:_
            2:-------------------
            3:-------------------
            '1:_
            */

            var expression = Apply(Implicit("a", Type), Implicit("b", Type));
            Assert.AreEqual("(a:? b:?):_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:('2:* -> '1:_):(* -> _) b:'2:*):'1:_", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply1_2()
        {
            var context = Terrain.Create();

            /*
            Apply 1:
            a:? b:?
            (a:? b:?):?
            1:-------------------
            (a:? b:?):'1:*
            (a:? b:'2:*):'1:*
            (a:('2:* -> '1:*) b:'2:*):'1:*
            2:-------------------
            3:-------------------
            '1:*
            */

            var expression = Apply(Implicit("a", Type), Implicit("b", Type), Type);
            Assert.AreEqual("(a:? b:?):?", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:('2:* -> '1:*):(* -> *) b:'2:*):'1:*", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply2_1()
        {
            var context = Terrain.Create();

            /*
            Apply 2:
            a:? b:System.Int32:*
            (a:? b:System.Int32:*):_
            1:-------------------
            (a:? b:System.Int32:*):'1:_
            (a:(System.Int32:* -> '1:_) b:System.Int32:*):'1:_
            2:-------------------
            3:-------------------
            '1:_
            */

            var expression = Apply(Implicit("a", Type), Implicit("b", Implicit("System.Int32", Kind)));
            Assert.AreEqual("(a:? b:System.Int32:*):_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:(System.Int32:* -> '1:_):(* -> _) b:System.Int32:*):'1:_", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply2_2()
        {
            var context = Terrain.Create();

            /*
            Apply 2:
            a:? b:System.Int32:*
            (a:? b:System.Int32:*):?
            1:-------------------
            (a:? b:System.Int32:*):'1:*
            (a:(System.Int32:* -> '1:*) b:System.Int32:*):'1:*
            2:-------------------
            3:-------------------
            '1:*
            */

            var expression = Apply(Implicit("a", Type), Implicit("b", Implicit("System.Int32", Kind)), Type);
            Assert.AreEqual("(a:? b:System.Int32:*):?", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:(System.Int32:* -> '1:*):(* -> *) b:System.Int32:*):'1:*", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply3_1()
        {
            var context = Terrain.Create();

            /*
            Apply 3:
            a:(System.Int32:* -> ?) b:?
            (a:(System.Int32:* -> ?) b:?):_
            1:-------------------
            (a:(System.Int32:* -> ?) b:?):'1:_                   : Hint('1)
            (a:(System.Int32:* -> ?) b:'2:*):'1:_                : Hint('2)
            (a:(System.Int32:* -> '1:*) b:'2:*):'1:_             : Hint('2 -> '1), Memoize('2 => System.Int32)
            2:-------------------
            (a:(System.Int32:* -> '1:*) b:System.Int32:*):'1:*   : Update('2 => System.Int32)
            3:-------------------
            '1:*
            */

            var expression = Apply(Implicit("a", Lambda(Bound("System.Int32", Kind), Type)), Implicit("b", Type));
            Assert.AreEqual("(a:(System.Int32:* -> ?):_ b:?):_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:(System.Int32:* -> '1:*):(* -> *) b:System.Int32:*):'1:*", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply3_2()
        {
            var context = Terrain.Create();

            /*
            Apply 3:
            a:(System.Int32:* -> _) b:?
            (a:(System.Int32:* -> _) b:?):?
            1:-------------------
            (a:(System.Int32:* -> _) b:?):'1:*                   : Hint('1)
            (a:(System.Int32:* -> _) b:'2:*):'1:*                : Hint('2)
            (a:(System.Int32:* -> '1:*) b:'2:*):'1:*             : Hint('2 -> '1), Memoize('2 => System.Int32)
            2:-------------------
            (a:(System.Int32:* -> '1:*) b:System.Int32:*):'1:*   : Update('2 => System.Int32)
            3:-------------------
            '1:*
            */

            var expression = Apply(Implicit("a", Lambda(Bound("System.Int32", Kind), Unspecified)), Implicit("b", Type), Type);
            Assert.AreEqual("(a:(System.Int32:* -> _):_ b:?):?", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:(System.Int32:* -> '1:*):(* -> *) b:System.Int32:*):'1:*", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply3_3()
        {
            var context = Terrain.Create();

            /*
            Apply 3:
            a:(System.Int32:* -> ?) b:?
            (a:(System.Int32:* -> ?) b:?):?
            1:-------------------
            (a:(System.Int32:* -> ?) b:?):'1:*                   : Hint('1)
            (a:(System.Int32:* -> ?) b:'2:*):'1:*                : Hint('2)
            (a:(System.Int32:* -> '1:*) b:'2:*):'1:*             : Hint('2 -> '1), Memoize('2 => System.Int32)
            2:-------------------
            (a:(System.Int32:* -> '1:*) b:System.Int32:*):'1:*   : Update('2 => System.Int32)
            3:-------------------
            '1:*
            */

            var expression = Apply(Implicit("a", Lambda(Bound("System.Int32", Kind), Type)), Implicit("b", Type), Type);
            Assert.AreEqual("(a:(System.Int32:* -> ?):_ b:?):?", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:(System.Int32:* -> '1:*):(* -> *) b:System.Int32:*):'1:*", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply3_4()
        {
            var context = Terrain.Create();

            /*
            Apply 3:
            a:(System.Int32:* -> _) b:?
            (a:(System.Int32:* -> _):* b:?):_
            1:-------------------
            (a:(System.Int32:* -> _):* b:?):'1:_                   : Hint('1)
            (a:(System.Int32:* -> _):* b:'2:*):'1:_                : Hint('2)
            (a:(System.Int32:* -> _):(* -> *) b:'2:*):'1:_             : Hint('2 -> '1)
            (a:(System.Int32:* -> '1:*):(* -> *) b:'2:*):'1:_          : Memoize('2 => System.Int32)
            2:-------------------
            (a:(System.Int32:* -> '1:*):(* -> *) b:System.Int32:*):'1:*        : Update('2 => System.Int32)
            3:-------------------
            '1:*
            */

            var expression = Apply(Implicit("a", Lambda(Bound("System.Int32", Kind), Unspecified)), Implicit("b", Type));
            Assert.AreEqual("(a:(System.Int32:* -> _):_ b:?):_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:(System.Int32:* -> '1:_):(* -> _) b:System.Int32:*):'1:_", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply4()
        {
            var context = Terrain.Create();

            /*
            Apply 4:
            a b c
            ((a:_ b:_):_ c:_):_
            1:-------------------
            ((a:_ b:_):_ c:_):'1
            ((a:_ b:_):_ c:'2):'1
            ((a:_ b:_):('2 -> '1) c:'2):'1
            ((a:_ b:'3):('2 -> '1) c:'2):'1
            ((a:('3 -> ('2 -> '1)) b:'3):('2 -> '1) c:'2):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Apply(Implicit("a"), Implicit("b")), Implicit("c"));
            Assert.AreEqual("((a:_ b:_):_ c:_):_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("((a:('3:_ -> ('2:_ -> '1:_):(_ -> _)):(_ -> (_ -> _)) b:'3:_):('2:_ -> '1:_):(_ -> _) c:'2:_):'1:_", inferred.AnnotatedReadableString);
        }
#if false
        [Test]
        public void Apply5()
        {
            var context = Environment.Create();

            /*
            Apply 5:
            a b c:System.Int32
            ((a:_ b:_):_ c:System.Int32):_
            1:-------------------
            ((a:_ b:_):_ c:System.Int32):'1
            ((a:_ b:_):(System.Int32 -> '1) c:System.Int32):'1
            ((a:_ b:'2):(System.Int32 -> '1) c:System.Int32):'1
            ((a:('2 -> (System.Int32 -> '1)) b:'2):(System.Int32 -> '1) c:System.Int32):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Apply(Implicit("a"), Implicit("b")), Implicit("c", Implicit("System.Int32")));
            Assert.AreEqual("((a:_ b:_):_ c:System.Int32:_):_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("((a:('2:_ -> (System.Int32:_ -> '1:_):(_ -> _):_):(_ -> (_ -> _):_):_ b:'2:_):(System.Int32:_ -> '1:_):(_ -> _):_ c:System.Int32:_):'1:_", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply6()
        {
            var context = Environment.Create();

            /*
            Apply 6:
            a b:System.Int32 c
            ((a:_ b:System.Int32):_ c:_):_
            1:-------------------
            ((a:_ b:System.Int32):_ c:_):'1
            ((a:_ b:System.Int32):_ c:'2):'1
            ((a:_ b:System.Int32):('2 -> '1) c:'2):'1
            ((a:(System.Int32 -> ('2 -> '1)) b:System.Int32):('2 -> '1) c:'2):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Apply(Implicit("a"), Implicit("b", Implicit("System.Int32"))), Implicit("c"));
            Assert.AreEqual("((a:_ b:System.Int32:_):_ c:_):_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("((a:(System.Int32:_ -> ('2:_ -> '1:_):(_ -> _):_):(_ -> (_ -> _):_):_ b:System.Int32:_):('2:_ -> '1:_):(_ -> _):_ c:'2:_):'1:_", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply7()
        {
            var context = Environment.Create();

            /*
            Apply 7:
            a:(System.Int32 -> _) b c
            ((a:(System.Int32 -> _) b:_):_ c:_):_
            1:-------------------
            ((a:(System.Int32 -> _) b:_):_ c:_):'1                                 : Hint('1)
            ((a:(System.Int32 -> _) b:_):_ c:'2):'1                                : Hint('2)
            ((a:(System.Int32 -> _) b:_):('2 -> '1) c:'2):'1                       : Hint(('2 -> '1))
            ((a:(System.Int32 -> _) b:'3):('2 -> '1) c:'2):'1                      : Hint('3)
            ((a:(System.Int32 -> ('2 -> '1)) b:'3):('2 -> '1) c:'2):'1             : Hint('3 -> ('2 -> '1)), Memoize('3 => System.Int32)
            2:-------------------
            ((a:(System.Int32 -> ('2 -> '1)) b:System.Int32):('2 -> '1) c:'2):'1   : Update('3 => System.Int32)
            3:-------------------
            '1
            */

            var expression = Apply(Apply(Implicit("a", Lambda(Bound("System.Int32"), Unspecified)), Implicit("b")), Implicit("c"));
            Assert.AreEqual("((a:(System.Int32:_ -> _):_ b:_):_ c:_):_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("((a:(System.Int32:_ -> ('2:_ -> '1:_):(_ -> _):_):(_ -> (_ -> _):_):_ b:System.Int32:_):('2:_ -> '1:_):(_ -> _):_ c:'2:_):'1:_", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply8()
        {
            var context = Environment.Create();

            /*
            Apply 8:
            a (b c)
            (a:_ (b:_ c:_):_):_
            1:-------------------
            (a:_ (b:_ c:_):_):'1
            (a:_ (b:_ c:_):'2):'1
            (a:('2 -> '1) (b:_ c:_):'2):'1
            (a:('2 -> '1) (b:_ c:'3):'2):'1
            (a:('2 -> '1) (b:('3 -> '2) c:'3):'2):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Implicit("a"), Apply(Implicit("b"), Implicit("c")));
            Assert.AreEqual("(a:_ (b:_ c:_):_):_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:('2:_ -> '1:_):(_ -> _):_ (b:('3:_ -> '2:_):(_ -> _):_ c:'3:_):'2:_):'1:_", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply9()
        {
            var context = Environment.Create();

            /*
            Apply 9:
            (a (b c)):System.Int32
            (a:_ (b:_ c:_):_):System.Int32
            1:-------------------
            (a:_ (b:_ c:_):'1):System.Int32
            (a:_ (b:_ c:_):'1):System.Int32
            (a:_ (b:_ c:'2):'1):System.Int32
            (a:_ (b:('2 -> '1) c:'2):'1):System.Int32
            (a:('1 -> System.Int32) (b:('2 -> '1) c:'2):'1):System.Int32
            2:-------------------
            3:-------------------
            System.Int32
            */

            var expression = Apply(Implicit("a"), Apply(Implicit("b"), Implicit("c")), Implicit("System.Int32"));
            Assert.AreEqual("(a:_ (b:_ c:_):_):System.Int32:_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:('1:_ -> System.Int32:_):(_ -> _):_ (b:('2:_ -> '1:_):(_ -> _):_ c:'2:_):'1:_):System.Int32:_", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply10()
        {
            var context = Environment.Create();

            /*
            Apply 10:
            a (b c):System.Int32
            (a:_ (b:_ c:_):System.Int32):_
            1:-------------------
            (a:_ (b:_ c:_):System.Int32):'1
            (a:_ (b:_ c:'2):System.Int32):'1
            (a:_ (b:('2 -> System.Int32) c:'2):System.Int32):'1
            (a:(System.Int32 -> '1) (b:('2 -> System.Int32) c:'2):System.Int32):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Implicit("a"), Apply(Implicit("b"), Implicit("c"), Implicit("System.Int32")));
            Assert.AreEqual("(a:_ (b:_ c:_):System.Int32:_):_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:(System.Int32:_ -> '1:_):(_ -> _):_ (b:('2:_ -> System.Int32:_):(_ -> _):_ c:'2:_):System.Int32:_):'1:_", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply11()
        {
            var context = Environment.Create();

            /*
            Apply 11:
            a (b c:System.Int32)
            (a:_ (b:_ c:System.Int32):_):_
            1:-------------------
            (a:_ (b:_ c:System.Int32):_):'1
            (a:_ (b:_ c:System.Int32):'2):'1
            (a:_ (b:(System.Int32 -> '2) c:System.Int32):'2):'1
            (a:('2 -> '1) (b:(System.Int32 -> '2) c:System.Int32):'2):'1
            2:-------------------
            3:-------------------
            '1
            */

            var expression = Apply(Implicit("a"), Apply(Implicit("b"), Implicit("c", Implicit("System.Int32"))));
            Assert.AreEqual("(a:_ (b:_ c:System.Int32:_):_):_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:('2:_ -> '1:_):(_ -> _):_ (b:(System.Int32:_ -> '2:_):(_ -> _):_ c:System.Int32:_):'2:_):'1:_", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply12()
        {
            var context = Environment.Create();

            /*
            Apply 12:
            a (b:(System.Int32 -> _) c)
            (a:_ (b:(System.Int32 -> _) c:_):_):_
            1:-------------------
            (a:_ (b:(System.Int32 -> _) c:_):_):'1
            (a:_ (b:(System.Int32 -> _) c:_):'2):'1
            (a:_ (b:(System.Int32 -> _) c:'3):'2):'1
            (a:_ (b:(System.Int32 -> '2) c:'3):'2):'1                           : Memoize('3 => System.Int32)
            (a:('2 -> '1) (b:(System.Int32 -> '2) c:'3):'2):'1
            2:-------------------
            (a:('2 -> '1) (b:(System.Int32 -> '2) c:System.Int32):'2):'1                 : Update('3 => System.Int32)
            3:-------------------
            '1
            */

            var expression = Apply(Implicit("a"), Apply(Implicit("b", Lambda(Bound("System.Int32"), Unspecified)), Implicit("c")));
            Assert.AreEqual("(a:_ (b:(System.Int32:_ -> _):_ c:_):_):_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:('2:_ -> '1:_):(_ -> _):_ (b:(System.Int32:_ -> '2:_):(_ -> _):_ c:System.Int32:_):'2:_):'1:_", inferred.AnnotatedReadableString);
        }

        [Test]
        public void Apply13()
        {
            var context = Environment.Create();

            /*
            Apply 13:
            a:(System.Int32 -> _ -> _) (b c)
            (a:((System.Int32 -> _) -> _) (b:_ c:_):_):_
            1:-------------------
            (a:((System.Int32 -> _) -> _) (b:_ c:_):_):'1
            (a:((System.Int32 -> _) -> _) (b:_ c:_):'2):'1
            (a:((System.Int32 -> _) -> _) (b:_ c:'3):'2):'1
            (a:((System.Int32 -> _) -> _) (b:('3 -> '2) c:'3):'2):'1
            (a:((System.Int32 -> '4) -> '1) (b:('3 -> '2) c:'3):'2):'1                       : Memoize('2 => (System.Int32 -> '4))
            2:-------------------
            (a:((System.Int32 -> '4) -> '1) (b:('3 -> '2) c:'3):(System.Int32 -> '4)):'1     : Update('2 => (System.Int32 -> '4))
            (a:((System.Int32 -> '4) -> '1) (b:('3 -> (System.Int32 -> '4)) c:'3):(System.Int32 -> '4)):'1     : Update('2 => (System.Int32 -> '4))
            */

            var expression = Apply(Implicit("a", Lambda(Lambda(Bound("System.Int32"), Unspecified), Unspecified)), Apply(Implicit("b"), Implicit("c")));
            Assert.AreEqual("(a:((System.Int32:_ -> _):_ -> _):_ (b:_ c:_):_):_", expression.StrictReadableString);

            var (inferred, errors) = context.Infer(expression, Expression.Unspecified);
            Assert.AreEqual("(a:((System.Int32:_ -> '4:_):(_ -> _):_ -> '1:_):((_ -> _):_ -> _):_ (b:('3:_ -> (System.Int32:_ -> '4:_):(_ -> _):_):(_ -> _):_ c:'3:_):(System.Int32:_ -> '4:_):(_ -> _):_):'1:_", inferred.AnnotatedReadableString);
        }
#endif
    }
}