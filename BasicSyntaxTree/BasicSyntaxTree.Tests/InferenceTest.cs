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
            var integerExpression = UntypedExpression.Constant(123);
            var actual = integerExpression.Infer(globalEnv);

            // Int32
            Assert.AreEqual("Int32", actual.Type.ToString());
        }

        [Test]
        public void StringExpression()
        {
            var globalEnv = Expression.CreateEnvironment();

            // "ABC"
            var stringExpression = UntypedExpression.Constant("ABC");
            var actual = stringExpression.Infer(globalEnv);

            // String
            Assert.AreEqual("String", actual.Type.ToString());
        }

        [Test]
        public void FunctionExpression()
        {
            var globalEnv = Expression.CreateEnvironment(
                // (+) = Int32 -> (Int32 -> Int32)
                ("+", Type.Function(Type.Dotnet<int>(), Type.Function(Type.Dotnet<int>(), Type.Dotnet<int>())))
                );

            // fun x = ((+) x) 1
            var inner = UntypedExpression.Apply(UntypedExpression.Variable("+"), UntypedExpression.Variable("x"));
            var outer = UntypedExpression.Apply(inner, UntypedExpression.Constant(1));
            var fun = UntypedExpression.Lambda("x", outer);
            var actual = fun.Infer(globalEnv);

            // Int32 -> Int32
            Assert.AreEqual("Int32 -> Int32", actual.Type.ToString());
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
