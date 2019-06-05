using BasicSyntaxTree.Expressions;
using BasicSyntaxTree.Expressions.Untyped;
using BasicSyntaxTree.Types;
using NUnit.Framework;

namespace BasicSyntaxTree
{
    [TestFixture]
    public sealed class InferenceTest
    {
        private static readonly System.Uri target = new System.Uri("", System.UriKind.RelativeOrAbsolute);

        [Test]
        public void IntegerExpression()
        {
            var globalEnv = Expression.CreateEnvironment();

            // 123
            var textRegion = TextRegion.Create(target, 1, 1, 5, 1);
            var integerExpression = UntypedExpression.Constant(123, textRegion);
            var actual = integerExpression.Infer(globalEnv);

            // Int32
            Assert.AreEqual("Int32", actual.Type.ToString());
        }

        [Test]
        public void StringExpression()
        {
            var globalEnv = Expression.CreateEnvironment();

            // "ABC"
            var textRegion = TextRegion.Create(target, 1, 1, 5, 1);
            var stringExpression = UntypedExpression.Constant("ABC", textRegion);
            var actual = stringExpression.Infer(globalEnv);

            // String
            Assert.AreEqual("String", actual.Type.ToString());
        }

        [Test]
        public void FunctionExpression()
        {
            var globalEnv = Expression.CreateEnvironment(
                // (+) = Int32 -> (Int32 -> Int32)
                ("+", Type.Function(Type.ClsType<int>(), Type.Function(Type.ClsType<int>(), Type.ClsType<int>())))
                );

            // fun x = ((+) x) 1
            var textRegion = TextRegion.Create(target, 1, 1, 5, 1);
            var inner = UntypedExpression.Apply(UntypedExpression.Variable("+", textRegion), UntypedExpression.Variable("x", textRegion), textRegion);
            var outer = UntypedExpression.Apply(inner, UntypedExpression.Constant(1, textRegion), textRegion);
            var fun = UntypedExpression.Lambda("x", outer, textRegion);
            var actual = fun.Infer(globalEnv);

            // Int32 -> Int32
            Assert.AreEqual("Int32 -> Int32", actual.Type.ToString());
        }

        [Test]
        public void FunctionCombinedExpression()
        {
            var globalEnv = Expression.CreateEnvironment();

            // fun f = g => x => f (g x)
            var textRegion = TextRegion.Create(target, 1, 1, 5, 1);
            var expra2 = UntypedExpression.Variable("x", textRegion);
            var exprf2 = UntypedExpression.Variable("g", textRegion);
            var expra1 = UntypedExpression.Apply(exprf2, expra2, textRegion);
            var exprf1 = UntypedExpression.Variable("f", textRegion);
            var expr3 = UntypedExpression.Apply(exprf1, expra1, textRegion);
            var expr2 = UntypedExpression.Lambda("x", expr3, textRegion);
            var expr1 = UntypedExpression.Lambda("g", expr2, textRegion);
            var fun = UntypedExpression.Lambda("f", expr1, textRegion);
            var actual = fun.Infer(globalEnv);

            Assert.AreEqual("('d -> 'e) -> ('c -> 'd) -> 'c -> 'e", actual.Type.ToString());
        }
    }
}
