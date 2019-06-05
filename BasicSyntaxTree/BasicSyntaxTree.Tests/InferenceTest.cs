using BasicSyntaxTree.Expressions;
using BasicSyntaxTree.Expressions.Untyped;
using BasicSyntaxTree.Types;
using NUnit.Framework;

namespace BasicSyntaxTree
{
    [TestFixture]
    public sealed class InferenceTest
    {
        [Test]
        public void IntegerExpression()
        {
            var globalEnv = Expression.CreateEnvironment();

            // 123
            var nat = UntypedExpression.Constant(123);
            var actual = nat.Infer(globalEnv);

            // Integer
            Assert.AreEqual("Integer", actual.Type.ToString());
        }

        [Test]
        public void FunctionExpression()
        {
            var globalEnv = Expression.CreateEnvironment(
                // (+) = Integer -> (Integer -> Integer)
                ("+", Type.Function(Type.Integer(), Type.Function(Type.Integer(), Type.Integer())))
                );

            // fun x = ((+) x) 1
            var inner = UntypedExpression.Apply(UntypedExpression.Variable("+"), UntypedExpression.Variable("x"));
            var outer = UntypedExpression.Apply(inner, UntypedExpression.Constant(1));
            var fun = UntypedExpression.Lambda("x", outer);
            var actual = fun.Infer(globalEnv);

            // Integer -> Integer
            Assert.AreEqual("Integer -> Integer", actual.Type.ToString());
        }

        [Test]
        public void FunctionCombinedExpression()
        {
            var globalEnv = Expression.CreateEnvironment();

            // fun f = g => x => f (g x)
            var expra2 = UntypedExpression.Variable("x");
            var exprf2 = UntypedExpression.Variable("g");
            var expra1 = UntypedExpression.Apply(exprf2, expra2);
            var exprf1 = UntypedExpression.Variable("f");
            var expr3 = UntypedExpression.Apply(exprf1, expra1);
            var expr2 = UntypedExpression.Lambda("x", expr3);
            var expr1 = UntypedExpression.Lambda("g", expr2);
            var fun = UntypedExpression.Lambda("f", expr1);
            var actual = fun.Infer(globalEnv);

            Assert.AreEqual("('d -> 'e) -> ('c -> 'd) -> 'c -> 'e", actual.Type.ToString());
        }
    }
}
