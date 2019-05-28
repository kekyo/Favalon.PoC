using NUnit.Framework;
using System.Linq;

using TypeInferences.Expressions;
using TypeInferences.Types;

namespace TypeInferences
{
    [TestFixture]
    public sealed class AvalonExpressionTest
    {
        #region Constant
        [Test]
        public void Int32ConstantIsInt32Type()
        {
            // var a = 123;
            var constant = AvalonExpression.Constant(123);
            Assert.AreEqual(Int32Type.Instance, constant.InferenceType);
        }

        [Test]
        public void DoubleConstantIsDoubleType()
        {
            // var a = 123.456;
            var constant = AvalonExpression.Constant(123.456);
            Assert.AreEqual(DoubleType.Instance, constant.InferenceType);
        }

        [Test]
        public void StringConstantIsStringType()
        {
            // var a = "ABC";
            var constant = AvalonExpression.Constant("ABC");
            Assert.AreEqual(StringType.Instance, constant.InferenceType);
        }
        #endregion

        #region Increment
        [Test]
        public void Int32ConstantIncrementToInt32Constant()
        {
            // var a = Increment(123);
            var constant = AvalonExpression.Constant(123);
            var apply = AvalonExpression.Increment(constant);
            Assert.AreEqual(Int32Type.Instance, constant.InferenceType);
        }

        [Test]
        public void DoubleConstantIncrementToDoubleConstant()
        {
            // var a = Increment(123.456);
            var constant = AvalonExpression.Constant(123.456);
            var apply = AvalonExpression.Increment(constant);
            Assert.AreEqual(DoubleType.Instance, constant.InferenceType);
        }

        #endregion

        #region Lambda
        [Test]
        public void Int32ConstantFromInt32TypeLambda()
        {
            // var f = 123 => 456;
            var parameter = AvalonExpression.Constant(123);
            var lambda = AvalonExpression.Lambda(AvalonExpression.Constant(456), parameter);
            Assert.AreEqual(Int32Type.Instance, lambda.InferenceType);
        }

        [Test]
        public void DoubleConstantFromInt32TypeLambda()
        {
            // var f = 123 => 456.789;
            var parameter = AvalonExpression.Constant(123);
            var lambda = AvalonExpression.Lambda(AvalonExpression.Constant(456.789), parameter);
            Assert.AreEqual(DoubleType.Instance, lambda.InferenceType);
        }

        [Test]
        public void StringConstantFromInt32TypeLambda()
        {
            // var f = 123 => "ABC";
            var parameter = AvalonExpression.Constant(123);
            var lambda = AvalonExpression.Lambda(AvalonExpression.Constant("ABC"), parameter);
            Assert.AreEqual(StringType.Instance, lambda.InferenceType);
        }
        #endregion
    }
}
