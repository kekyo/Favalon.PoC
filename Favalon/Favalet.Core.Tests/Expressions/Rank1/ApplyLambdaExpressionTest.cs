using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Rank1
{
    using static StaticFactories;

    [TestFixture]
    public sealed class ApplyLambdaExpressionTest
    {
        [Test]
        public void ApplyLambda1()
        {
            var environment = Environment.Create();

            /*
            Apply and Lambda 1:
            a -> a b
            (a:_ -> (a:_ b:_):_):_
            1:-------------------
            (a:_ -> (a:_ b:_):_):'1
            (a:'2 -> (a:_ b:_):_):'1                          : Bind(a:'2)
            (a:'2 -> (a:_ b:_):'3):'1                         : Memoize('1 => ('2 -> '3))
            (a:'2 -> (a:_ b:'4):'3):'1
            (a:'2 -> (a:('4 -> '3) b:'4):'3):'1               : Lookup(a => '2), Memoize('2 => ('4 -> '3)))
            2:-------------------
            (a:('4 -> '3) -> (a:('4 -> '3) b:'4):'3):'1       : Update('2 => ('4 -> '3)))
            (a:('4 -> '3) -> (a:('4 -> '3) b:'4):'3):('2 -> '3)       : Update('1 => ('2 -> '3))
            (a:('4 -> '3) -> (a:('4 -> '3) b:'4):'3):(('4 -> '3) -> '3)       : Update('2 => ('4 -> '3)))
            3:-------------------
            ('4 -> '3) -> '3
            */

            var expression = Lambda(Bound("a"), Apply(Free("a"), Implicit("b")));
            Assert.AreEqual("(a:_ -> (a:_ b:_):_):_", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:('4 -> '3) -> (a:('4 -> '3) b:'4):'3):(('4 -> '3) -> '3)", inferred.StrictReadableString);
        }

        [Test]
        public void ApplyLambda2()
        {
            var environment = Environment.Create();

            /*
            Apply and Lambda 2:
            a -> b -> a b
            (a:_ -> (b:_ -> (a:_ b:_):_):_):_
            1:-------------------
            (a:_ -> (b:_ -> (a:_ b:_):_):_):'1
            (a:'2 -> (b:_ -> (a:_ b:_):_):_):'1                       : Bind(a:'2)
            (a:'2 -> (b:_ -> (a:_ b:_):_):'3):'1                      : Memoize('1 => ('2 -> '3))
            (a:'2 -> (b:'4 -> (a:_ b:_):_):'3):'1                     : Bind(b:'4)
            (a:'2 -> (b:'4 -> (a:_ b:_):'5):'3):'1                    : Memoize('3 => ('4 -> '5))
            (a:'2 -> (b:'4 -> (a:_ b:'4):'5):'3):'1                   : Lookup(b => '4)
            (a:'2 -> (b:'4 -> (a:('4 -> '5) b:'4):'5):'3):'1          : Lookup(a => '2), Memoize('2 => ('4 -> '5))
            2:-------------------
            (a:'2 -> (b:'4 -> (a:('4 -> '5) b:'4):'5):('4 -> '5)):'1                  : Update('3 => ('4 -> '5))
            (a:('4 -> '5) -> (b:'4 -> (a:('4 -> '5) b:'4):'5):('4 -> '5)):'1          : Update('2 => ('4 -> '5))
            (a:('4 -> '5) -> (b:'4 -> (a:('4 -> '5) b:'4):'5):('4 -> '5)):('2 -> '3)  : Update('1 => ('2 -> '3))
            (a:('4 -> '5) -> (b:'4 -> (a:('4 -> '5) b:'4):'5):('4 -> '5)):(('4 -> '5) -> '3)            : Update('2 => ('4 -> '5))
            (a:('4 -> '5) -> (b:'4 -> (a:('4 -> '5) b:'4):'5):('4 -> '5)):(('4 -> '5) -> ('4 -> '5))    : Update('3 => ('4 -> '5))
            3:-------------------
            ('4 -> '5) -> ('4 -> '5)
            ('4 -> '5) -> '4 -> '5
            */

            var expression = Lambda(Bound("a"), Lambda(Bound("b"), Apply(Free("a"), Free("b"))));
            Assert.AreEqual("(a:_ -> (b:_ -> (a:_ b:_):_):_):_", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:('4 -> '5) -> (b:'4 -> (a:('4 -> '5) b:'4):'5):('4 -> '5)):(('4 -> '5) -> ('4 -> '5))", inferred.StrictReadableString);
        }
    }
}