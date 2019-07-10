using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalet.Expressions.Variable
{
    using static StaticFactories;

    [TestFixture]
    public sealed class LiteralExpressionTest
    {
        [Test]
        public void Literal1()
        {
            var environment = Environment.Create();

            /*
            Literal1:
            123
            123:_
            1:-------------------
            123:Numeric
            2:-------------------
            3:-------------------
            Numeric
             */

            var expression = Literal(123);
            Assert.AreEqual("123:?:*", expression.StrictReadableString);

            var inferred = environment.Infer<Expression>(expression);
            Assert.AreEqual("123:Numeric:*", inferred.StrictReadableString);
        }

        [Test]
        public void Literal2()
        {
            var environment = Environment.Create();

            /*
            Literal2:
            "ABC"
            "ABC":_
            1:-------------------
            "ABC":System.String
            2:-------------------
            3:-------------------
            System.String
             */

            var expression = Literal("ABC");
            Assert.AreEqual("\"ABC\":?:*", expression.StrictReadableString);

            var inferred = environment.Infer<Expression>(expression);
            Assert.AreEqual("\"ABC\":System.String:*", inferred.StrictReadableString);
        }
    }
}
