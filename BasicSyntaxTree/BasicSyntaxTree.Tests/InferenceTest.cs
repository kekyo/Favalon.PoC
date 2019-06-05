using BasicSyntaxTree.Expressions;
using BasicSyntaxTree.Types;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace BasicSyntaxTree
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
            Assert.AreEqual("Integer", actual.ToString());
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
            var fun = Expression.Lambda("x", outer);
            var actual = fun.Infer(globalEnv);

            // Integer -> Integer
            Assert.AreEqual("Integer -> Integer", actual.ToString());
        }

        [Test]
        public void FunctionCombinedExpression()
        {
            var globalEnv = CreateEnvironment();

            // fun f = g => x => f (g x)
            var expra2 = Expression.Variable("x");
            var exprf2 = Expression.Variable("g");
            var expra1 = Expression.Apply(exprf2, expra2);
            var exprf1 = Expression.Variable("f");
            var expr3 = Expression.Apply(exprf1, expra1);
            var expr2 = Expression.Lambda("x", expr3);
            var expr1 = Expression.Lambda("g", expr2);
            var fun = Expression.Lambda("f", expr1);
            var actual = fun.Infer(globalEnv);

            Assert.AreEqual("('T3 -> 'T4) -> ('T2 -> 'T3) -> 'T2 -> 'T4", actual.ToString());
        }
    }
}
