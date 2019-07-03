using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    using static StaticFactories;

    [TestFixture]
    public sealed class ApplyExpressionTest
    {
        [Test]
        public void Apply1()
        {
            var environment = Environment.Create();

            //a b
            //(a:? b:?):?
            //1:-------------------
            //(a:? b :?):'1
            //(a:? b : '2):'1
            //(a: ('2 -> '1) b: '2):'1
            //2:-------------------
            //3:-------------------
            //'1
            var expression = Apply(Variable("a"), Variable("b"));

            var inferred = environment.Infer(expression);

            Assert.AreEqual("'1", inferred.HigherOrder.ReadableString);
        }
    }
}
