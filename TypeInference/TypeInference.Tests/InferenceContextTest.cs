using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeInference
{
    [TestFixture]
    public sealed class InferenceContextTest
    {
        [Test]
        public void Int32ConstantIsInt32Type()
        {
            // var a = 123;
            var expression = new ConstantExpression(123);
            Assert.IsTrue(expression.CalculatedTypes.SequenceEqual(new[] { typeof(int) }));
        }

        [Test]
        public void Int32ConstantPlusInt32ConstantIsInt32Type()
        {
            // var a = 123 + 456;
            var expression1 = new ConstantExpression(123);
            var expression2 = new ConstantExpression(456);
            var expression = new PlusOperatorExpression(expression1, expression2);
            Assert.IsTrue(expression.CalculatedTypes.SequenceEqual(new[] { typeof(int) }));
        }






        [Test]
        public void DoubleConstantIsDoubleType()
        {
            // var a = 123.456;
            var expression = new ConstantExpression(123.456);
            Assert.IsTrue(expression.CalculatedTypes.SequenceEqual(new[] { typeof(double) }));
        }

        [Test]
        public void StringConstantIsStringType()
        {
            // var a = "ABC";
            var expression = new ConstantExpression("ABC");
            Assert.IsTrue(expression.CalculatedTypes.SequenceEqual(new[] { typeof(string) }));
        }
    }
}
