using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BasicSyntaxTree.Tests
{
    [TestFixture]
    public sealed class InferenceTest
    {
        private static IReadOnlyDictionary<string, Type> CreateEnvironment(
            params (string name, Type type)[] environments) =>
            environments.ToDictionary(entry => entry.name, entry => entry.type);

        [Test]
        public void IntegerExpression()
        {
            var globalEnv = CreateEnvironment();

            // 123
            var nat = Expression.Natural(123);
            var actual = nat.Infer(globalEnv);

            // Integer
            var expected = Type.Integer;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FunctionExpression()
        {
            var globalEnv = CreateEnvironment(
                // (+) = Integer -> (Integer -> Integer)
                ("+", Type.Function(Type.Integer, Type.Function(Type.Integer, Type.Integer)))
                );

            // fun x = ((+) x) 1
            var inner = Expression.Apply(Expression.Variable("+"), Expression.Variable("x"));
            var outer = Expression.Apply(inner, Expression.Natural(1));
            var fun = Expression.Function("x", outer);
            var actual = fun.Infer(globalEnv);

            // Integer -> Integer
            var expected = Type.Function(Type.Integer, Type.Integer);
            Assert.AreEqual(expected, actual);
        }
    }
}
