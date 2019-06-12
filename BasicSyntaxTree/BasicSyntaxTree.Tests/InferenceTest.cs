using BasicSyntaxTree.Types;
using NUnit.Framework;
using System.Collections.Generic;

namespace BasicSyntaxTree
{
    using static BasicSyntaxTree.Expressions.UnresolvedExpression;
    using static BasicSyntaxTree.Types.Type;

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
            Assert.AreEqual("int", actual.InferredType.ToString());
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
            Assert.AreEqual("string", actual.InferredType.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void FunctionApplyAtEnvironmentExpression()
        {
            var globalEnv = new Environment();

            // (+) : int -> (int -> int)
            globalEnv.RegisterVariable("+", Function(RuntimeType<int>(), Function(RuntimeType<int>(), RuntimeType<int>())));

            // fun x -> (+) x 1
            // Lambda(x, Apply(Apply(+, x), 1))
            var inner = Apply("+", "x");
            var outer = Apply(inner, Constant(1));
            var fun = Lambda("x", outer);
            var actual = fun.Infer(globalEnv);

            // int -> int
            Assert.AreEqual("int -> int", actual.InferredType.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void FunctionExpression()
        {
            var globalEnv = new Environment();

            // fun f -> f 1
            // Lambda(f, Apply(f, 1))
            var expr = Apply("f", Constant(1));
            var fun = Lambda("f", expr);
            var actual = fun.Infer(globalEnv);

            Assert.AreEqual("(int -> 'b) -> 'b", actual.InferredType.ToString());
            Assert.IsFalse(actual.IsResolved);
        }

        [Test]
        public void FunctionCombinedExpression()
        {
            var globalEnv = new Environment();

            // fun f -> fun g -> f g
            // Lambda(f, Lambda(g, Apply(f, g)))
            var expr2 = Apply("f", "g");
            var expr1 = Lambda("g", expr2);
            var fun = Lambda("f", expr1);
            var actual = fun.Infer(globalEnv);

            Assert.AreEqual("('b -> 'c) -> 'b -> 'c", actual.InferredType.ToString());
            Assert.IsFalse(actual.IsResolved);
        }

        [Test]
        public void Function3CombinedExpression()
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

            Assert.AreEqual("('d -> 'e) -> ('c -> 'd) -> 'c -> 'e", actual.InferredType.ToString());
            Assert.IsFalse(actual.IsResolved);
        }

        [Test]
        public void FunctionInfereredExpression()
        {
            var globalEnv = new Environment();

            // fun a:int -> a
            // Lambda(a:int, a)
            var fun = Lambda(Variable("a", RuntimeType<int>()), "a");
            var actual = fun.Infer(globalEnv);

            Assert.AreEqual("int -> int", actual.InferredType.ToString());
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
            Assert.AreEqual("int", actual.InferredType.ToString());
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
            Assert.AreEqual("int", actual.InferredType.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void ApplyConstructorExpression()
        {
            var globalEnv = new Environment();

            globalEnv.RegisterVariable("Int32List", Function(RuntimeType<IEnumerable<int>>(), RuntimeType<List<int>>()));

            // fun xs -> Int32List xs
            // Lambda(xs, Apply(Int32List, xs))
            var inner = Apply("Int32List", "xs");
            var fun = Lambda("xs", inner);
            var actual = fun.Infer(globalEnv);

            // seq<int> -> List<int>
            Assert.AreEqual("seq<int> -> System.Collections.Generic.List<int>", actual.InferredType.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void ApplyConstructorWithAnnotationExpression()
        {
            var globalEnv = new Environment();

            globalEnv.RegisterVariable("Int32List", Function(RuntimeType<IEnumerable<int>>(), RuntimeType<List<int>>()));

            // fun xs -> (Int32List xs):List<Int32>
            // Lambda(xs, Apply(Int32List, xs, List<Int32>))
            var inner = Apply("Int32List", "xs", RuntimeType<List<int>>());
            var fun = Lambda("xs", inner);
            var actual = fun.Infer(globalEnv);

            // seq<int> -> List<int>
            Assert.AreEqual("seq<int> -> System.Collections.Generic.List<int>", actual.InferredType.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void ApplyTypeConstructorExpression()
        {
            var globalEnv = new Environment();

            // NewList: kind<List>
            globalEnv.RegisterVariable("List", KindType(typeof(List<>)));

            // Integer: kind<Integer>
            globalEnv.RegisterVariable("Int", KindType<int>());

            // fun xs -> List Int xs
            // Lambda(xs, Apply(Apply(NewList, int), xs))
            var inner = Apply("List", "Int");
            var outer = Apply(inner, "xs");
            var fun = Lambda("xs", outer);
            var actual = fun.Infer(globalEnv);

            // seq<'a> -> List<'a>
            Assert.AreEqual("seq<'a> -> System.Collections.Generic.List<'a>", actual.InferredType.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void AppliedTypeConstructorExpression()
        {
            var globalEnv = new Environment();

            // List: tycon
            // List: ty -> ty
            globalEnv.RegisterVariable("List", RuntimeType(typeof(List<>)));
            globalEnv.RegisterVariable("int", RuntimeType<int>());

            // List int
            // Apply(List, int)
            var expr = Apply("List", "int");
            var actual = expr.Infer(globalEnv);

            // List<int>
            Assert.AreEqual("System.Collections.Generic.List<int>", actual.InferredType.ToString());
            Assert.IsTrue(actual.IsResolved);
        }

        [Test]
        public void ApplySeqAppliedTypeConstructor()
        {
            var globalEnv = new Environment();

            globalEnv.RegisterVariable("List", RuntimeType(typeof(List<>)));
            globalEnv.RegisterVariable("int", RuntimeType<int>());

            // fun xs -> List int xs
            // Lambda(xs, Apply(Apply(List, int), xs))
            var inner = Apply("List", "int");
            var outer = Apply(inner, "xs");
            var fun = Lambda("xs", outer);
            var actual = fun.Infer(globalEnv);

            // int -> int
            Assert.AreEqual("int -> int", actual.InferredType.ToString());
            Assert.IsTrue(actual.IsResolved);
        }
    }
}
