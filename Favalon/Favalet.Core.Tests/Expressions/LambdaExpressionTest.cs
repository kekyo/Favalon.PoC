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
        public void Apply1()
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
    }
}