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

        [Test]
        public void ObjectConstantIsObjectType()
        {
            // var a = new object();
            var constant = AvalonExpression.Constant(new object());
            Assert.AreEqual(ObjectType.Instance, constant.InferenceType);
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
        public void Int32ConstantFromInt32ParameterLambda()
        {
            // var f = a:Int32 => 123;
            var parameter = AvalonExpression.Parameter("a", Int32Type.Instance);
            var lambda = AvalonExpression.Lambda(AvalonExpression.Constant(123), parameter);
            Assert.AreEqual(Int32Type.Instance, lambda.InferenceType);
        }

        [Test]
        public void DoubleConstantFromInt32ParameterLambda()
        {
            // var f = a:Int32 => 456.789;
            var parameter = AvalonExpression.Parameter("a", Int32Type.Instance);
            var lambda = AvalonExpression.Lambda(AvalonExpression.Constant(456.789), parameter);
            Assert.AreEqual(DoubleType.Instance, lambda.InferenceType);
        }

        [Test]
        public void StringConstantFromInt32ParameterLambda()
        {
            // var f = a:Int32 => "ABC";
            var parameter = AvalonExpression.Parameter("a", Int32Type.Instance);
            var lambda = AvalonExpression.Lambda(AvalonExpression.Constant("ABC"), parameter);
            Assert.AreEqual(StringType.Instance, lambda.InferenceType);
        }

        [Test]
        public void Int32ValueFromInt32ParameterLambda()
        {
            // var f = a:Int32 => a;
            var parameter = AvalonExpression.Parameter("a", Int32Type.Instance);
            var lambda = AvalonExpression.Lambda(parameter, parameter);
            Assert.AreEqual(Int32Type.Instance, lambda.InferenceType);
        }

        [Test]
        public void UnassignedValueFromUnassignedParameterLambda()
        {
            // var f = a => a;
            var parameter = AvalonExpression.Parameter("a");
            var lambda = AvalonExpression.Lambda(parameter, parameter);
            Assert.IsTrue(lambda.InferenceType is UnassignedType);
        }
        #endregion
    }
}
