using NUnit.Framework;
using System.Linq;

using TypeInferences.Expressions;
using TypeInferences.Types;

namespace TypeInferences
{
    [TestFixture]
    public sealed class AvalonExpressionTest
    {
        private static readonly AvalonType doubleType = AvalonType.FromClrType<double>();
        private static readonly AvalonType int32Type = AvalonType.FromClrType<int>();
        private static readonly AvalonType stringType = AvalonType.FromClrType<string>();
        private static readonly AvalonType objectType = AvalonType.FromClrType<object>();

        #region Constant
        [Test]
        public void Int32ConstantIsInt32Type()
        {
            // var a = 123;
            var constant = AvalonExpression.Constant(123);
            Assert.AreEqual(int32Type, constant.InferenceType);
        }

        [Test]
        public void DoubleConstantIsDoubleType()
        {
            // var a = 123.456;
            var constant = AvalonExpression.Constant(123.456);
            Assert.AreEqual(doubleType, constant.InferenceType);
        }

        [Test]
        public void StringConstantIsStringType()
        {
            // var a = "ABC";
            var constant = AvalonExpression.Constant("ABC");
            Assert.AreEqual(stringType, constant.InferenceType);
        }

        [Test]
        public void ObjectConstantIsObjectType()
        {
            // var a = new object();
            var constant = AvalonExpression.Constant(new object());
            Assert.AreEqual(objectType, constant.InferenceType);
        }
        #endregion

        #region Increment
        [Test]
        public void Int32ConstantIncrementToInt32Constant()
        {
            // var a = Increment(123);
            var constant = AvalonExpression.Constant(123);
            var apply = AvalonExpression.Increment(constant);
            Assert.AreEqual(int32Type, constant.InferenceType);
        }

        [Test]
        public void DoubleConstantIncrementToDoubleConstant()
        {
            // var a = Increment(123.456);
            var constant = AvalonExpression.Constant(123.456);
            var apply = AvalonExpression.Increment(constant);
            Assert.AreEqual(doubleType, constant.InferenceType);
        }
        #endregion

        #region Lambda
        [Test]
        public void Int32ConstantFromInt32ParameterLambda()
        {
            // var f = a:Int32 => 123;
            var parameter = AvalonExpression.Parameter("a", int32Type);
            var lambda = AvalonExpression.Lambda(AvalonExpression.Constant(123), parameter);
            Assert.AreEqual(int32Type, lambda.InferenceType);
        }

        [Test]
        public void DoubleConstantFromInt32ParameterLambda()
        {
            // var f = a:Int32 => 456.789;
            var parameter = AvalonExpression.Parameter("a", int32Type);
            var lambda = AvalonExpression.Lambda(AvalonExpression.Constant(456.789), parameter);
            Assert.AreEqual(doubleType, lambda.InferenceType);
        }

        [Test]
        public void StringConstantFromInt32ParameterLambda()
        {
            // var f = a:Int32 => "ABC";
            var parameter = AvalonExpression.Parameter("a", int32Type);
            var lambda = AvalonExpression.Lambda(AvalonExpression.Constant("ABC"), parameter);
            Assert.AreEqual(stringType, lambda.InferenceType);
        }

        [Test]
        public void Int32ValueFromInt32ParameterLambda()
        {
            // var f = a:Int32 => a;
            var parameter = AvalonExpression.Parameter("a", int32Type);
            var lambda = AvalonExpression.Lambda(parameter, parameter);
            Assert.AreEqual(int32Type, lambda.InferenceType);
        }

        [Test]
        public void UnassignedValueFromUnassignedParameterLambda()
        {
            // var f = a => a;
            var parameter = AvalonExpression.Parameter("a");
            var lambda = AvalonExpression.Lambda(parameter, parameter);
            Assert.AreEqual(AvalonTypes.Unspecified, lambda.InferenceType.Type);
        }
        #endregion
    }
}
