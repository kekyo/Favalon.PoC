using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TypeInference.Expressions;
using TypeInference.Types;

namespace TypeInference
{
    [TestFixture]
    public sealed class InferenceContextTest
    {
        [Test]
        public void Int32ConstantIsInt32Type()
        {
            // var a = 123;
            var expression = new Constant(123);
            Assert.IsTrue(expression.CalculatedTypes.SequenceEqual(new[] { AvalonType.Create<int>() }));
        }

        [Test]
        public void Int32ConstantPlusInt32ConstantIsInt32Type()
        {
            // var a = 123 + 456;
            var expression1 = new Constant(123);
            var expression2 = new Constant(456);
            var expression = new OperatorPlus(expression1, expression2);
            Assert.IsTrue(expression.CalculatedTypes.SequenceEqual(new[] { AvalonType.Create<int>() }));
        }

        [Test]
        public void Int32ConstantPlusDoubleConstantIsDoubleType()
        {
            // var a = 123 + 456;
            var expression1 = new Constant(123);
            var expression2 = new Constant(456.789);
            var expression = new OperatorPlus(expression1, expression2);
            Assert.IsTrue(expression.CalculatedTypes.SequenceEqual(new[] { AvalonType.Create<double>() }));
        }











        [Test]
        public void DoubleConstantIsDoubleType()
        {
            // var a = 123.456;
            var expression = new Constant(123.456);
            Assert.IsTrue(expression.CalculatedTypes.SequenceEqual(new[] { AvalonType.Create<double>() }));
        }

        [Test]
        public void StringConstantIsStringType()
        {
            // var a = "ABC";
            var expression = new Constant("ABC");
            Assert.IsTrue(expression.CalculatedTypes.SequenceEqual(new[] { AvalonType.Create<string>() }));
        }
    }
}
