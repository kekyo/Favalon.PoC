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
    }
}
