using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions
{
    using static Internals.StaticFactories;

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
            (a:? -> (a:? b:?):?):?
            1:-------------------
            (a:? -> (a:? b:?):?):'1
            (a:'2 -> (a:? b:?):?):'1                          : Bind(a:'2)
            (a:'2 -> (a:? b:?):'3):'1                         : Memoize('1 => ('2 -> '3))
            (a:'2 -> (a:? b:'4):'3):'1
            (a:'2 -> (a:('4 -> '3) b:'4):'3):'1               : Lookup(a => '2), Memoize('2 => ('4 -> '3)))
            2:-------------------
            (a:('4 -> '3) -> (a:('4 -> '3) b:'4):'3):'1       : Update('2 => ('4 -> '3)))
            (a:('4 -> '3) -> (a:('4 -> '3) b:'4):'3):('2 -> '3)       : Update('1 => ('2 -> '3))
            (a:('4 -> '3) -> (a:('4 -> '3) b:'4):'3):(('4 -> '3) -> '3)       : Update('2 => ('4 -> '3)))
            3:-------------------
            ('4 -> '3) -> '3
            */

            var expression = Lambda(Variable("a"), Apply(Variable("a"), Variable("b")));
            Assert.AreEqual("(a:? -> (a:? b:?):?):?", expression.StrictReadableString);

            var inferred = environment.Infer(expression);
            Assert.AreEqual("(a:('4 -> '3) -> (a:('4 -> '3) b:'4):'3):(('4 -> '3) -> '3)", inferred.StrictReadableString);
        }
    }
}