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
            var integerExpression = Expression.Constant(123, textRegion);
            var actual = integerExpression.Infer(globalEnv);

            // Int32
            Assert.AreEqual("Int32", actual.Type.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void StringExpression()
        {
            var globalEnv = Expression.CreateEnvironment();

            // "ABC"
            var textRegion = TextRegion.Create(target, 1, 1, 5, 1);
            var stringExpression = Expression.Constant("ABC", textRegion);
            var actual = stringExpression.Infer(globalEnv);

            // String
            Assert.AreEqual("String", actual.Type.ToString());
            Assert.IsTrue(actual.IsResolved);
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
            var inner = Expression.Apply(Expression.Variable("+", textRegion), Expression.Variable("x", textRegion), textRegion);
            var outer = Expression.Apply(inner, Expression.Constant(1, textRegion), textRegion);
            var fun = Expression.Lambda("x", outer, textRegion);
            var actual = fun.Infer(globalEnv);
            
            // Int32 -> Int32
            Assert.AreEqual("Int32 -> Int32", actual.Type.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void FunctionCombinedExpression()
        {
            var globalEnv = Expression.CreateEnvironment();

            // fun f = g => x => f (g x)
            var textRegion = TextRegion.Create(target, 1, 1, 5, 1);
            var expra2 = Expression.Variable("x", textRegion);
            var exprf2 = Expression.Variable("g", textRegion);
            var expra1 = Expression.Apply(exprf2, expra2, textRegion);
            var exprf1 = Expression.Variable("f", textRegion);
            var expr3 = Expression.Apply(exprf1, expra1, textRegion);
            var expr2 = Expression.Lambda("x", expr3, textRegion);
            var expr1 = Expression.Lambda("g", expr2, textRegion);
            var fun = Expression.Lambda("f", expr1, textRegion);
            var actual = fun.Infer(globalEnv);

            Assert.AreEqual("('d -> 'e) -> ('c -> 'd) -> 'c -> 'e", actual.Type.ToString());
            Assert.IsFalse(actual.IsResolved);
        }

        [Test]
        public void BindInt32Expression()
        {
            var globalEnv = Expression.CreateEnvironment();

            // let x = 123 in x
            var textRegion = TextRegion.Create(target, 1, 1, 5, 1);
            var int32Expression = Expression.Constant(123, textRegion);
            var bindExpression = Expression.Bind("x", int32Expression, Expression.Variable("x", textRegion), textRegion);
            var actual = bindExpression.Infer(globalEnv);

            // Int32
            Assert.AreEqual("Int32", actual.Type.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

    }
}
