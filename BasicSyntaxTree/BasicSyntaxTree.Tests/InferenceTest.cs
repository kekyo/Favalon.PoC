using BasicSyntaxTree.Types;
using NUnit.Framework;
using System.Collections.Generic;

namespace BasicSyntaxTree
{
    using static BasicSyntaxTree.Expressions.UnresolvedExpression;

    [TestFixture]
    public sealed class InferenceTest
    {
        [Test]
        public void IntegerExpression()
        {
            var globalEnv = new Environment();

            // 123
            var integerExpression = Constant(123);
            var actual = integerExpression.Infer(globalEnv);

            // Int32
            Assert.AreEqual("Int32", actual.Type.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void StringExpression()
        {
            var globalEnv = new Environment();

            // "ABC"
            var stringExpression = Constant("ABC");
            var actual = stringExpression.Infer(globalEnv);

            // String
            Assert.AreEqual("String", actual.Type.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void FunctionExpression()
        {
            var globalEnv = new Environment();

            // (+) : fun a -> fun b -> a b
            // (+) : Lambda(a, Lambda(b, Apply(a, b)))
            //var plusExpression = Lambda("a", Lambda("b", Apply("a", "b")));
            //var plusExpressionType = plusExpression.Infer(globalEnv).Type;
            //globalEnv.RegisterVariable("+", plusExpressionType);

            // (+) : Int32 -> (Int32 -> Int32)
            globalEnv.RegisterVariable("+", Type.Function(Type.ClrType<int>(), Type.Function(Type.ClrType<int>(), Type.ClrType<int>())));

            // fun x -> (+) x 1
            // Lambda(x, Apply(Apply(+, x), 1))
            var inner = Apply("+", "x");
            var outer = Apply(inner, Constant(1));
            var fun = Lambda("x", outer);
            var actual = fun.Infer(globalEnv);
            
            // Int32 -> Int32
            Assert.AreEqual("Int32 -> Int32", actual.Type.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void FunctionCombinedExpression()
        {
            var globalEnv = new Environment();

            // fun f -> fun g -> fun x -> f (g x)
            // Lambda(f, Lambda(g, Lambda(x, Apply(f, Apply(g, x)))))
            var expra1 = Apply("g", "x");
            var expr3 = Apply("f", expra1);
            var expr2 = Lambda("x", expr3);
            var expr1 = Lambda("g", expr2);
            var fun = Lambda("f", expr1);
            var actual = fun.Infer(globalEnv);

            Assert.AreEqual("('d -> 'e) -> ('c -> 'd) -> 'c -> 'e", actual.Type.ToString());
            Assert.IsFalse(actual.IsResolved);
        }

        [Test]
        public void FunctionInfereredExpression()
        {
            var globalEnv = new Environment();

            // fun a:int -> a
            // Lambda(a:int, a)
            var fun = Lambda(Variable("a", Type.ClrType<int>()), "a");
            var actual = fun.Infer(globalEnv);

            Assert.AreEqual("Int32 -> Int32", actual.Type.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void BindInt32Expression()
        {
            var globalEnv = new Environment();

            // let x = 123 in x
            // Bind(x, 123, x)
            var bindExpression = Bind("x", Constant(123), "x");
            var actual = bindExpression.Infer(globalEnv);

            // Int32
            Assert.AreEqual("Int32", actual.Type.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void BindInfereredExpression()
        {
            var globalEnv = new Environment();

            // let x = 123 in
            //   let y = x in y
            // Bind(x, 123, Bind(y, x, y))
            var bindExpression = Bind("x", Constant(123), Bind("y", "x", "y"));
            var actual = bindExpression.Infer(globalEnv);

            // Int32
            Assert.AreEqual("Int32", actual.Type.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void TypeFunctionExpression()
        {
            var globalEnv = new Environment();

            globalEnv.RegisterVariable("List", Type.ClrType(typeof(List<>)));

            // fun xs -> List int xs
            // Lambda(xs, Apply(Apply(List, int), xs))
            var inner = Apply("List", "int");
            var outer = Apply(inner, "xs");
            var fun = Lambda("xs", outer);
            var actual = fun.Infer(globalEnv);

            // Int32 -> Int32
            Assert.AreEqual("Int32 -> Int32", actual.Type.ToString());
            Assert.IsTrue(actual.IsResolved);
        }
    }
}
