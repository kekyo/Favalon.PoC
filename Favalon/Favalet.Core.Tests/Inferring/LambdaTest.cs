using Favalet.Contexts;
using Favalet.Expressions;
using NUnit.Framework;
using System;

using static Favalet.CLRGenerator;
using static Favalet.Generator;

namespace Favalet.Inferring
{
    [TestFixture]
    public sealed class LambdaTest
    {
        private static void AssertLogicalEqual(
            IExpression expression,
            IExpression expected,
            IExpression actual)
        {
            if (!ExpressionAssert.Equals(expected, actual))
            {
                Assert.Fail(
                    "Expression = {0}\r\nExpected   = {1}\r\nActual     = {2}",
                    expression.GetPrettyString(PrettyStringTypes.Readable),
                    expected.GetPrettyString(PrettyStringTypes.Readable),
                    actual.GetPrettyString(PrettyStringTypes.Readable));
            }
        }

        [Test]
        public void LambdaWithoutAnnotation1()
        {
            var environment = CLREnvironment();

            // a -> a
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"));

            var actual = environment.Infer(expression);

            // (a:'0 -> a:'0):('0 -> '0)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Lambda(
                    BoundSymbol("a", ph0),
                    Identity("a", ph0),
                    Function(ph0, ph0));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithoutAnnotation2()
        {
            var environment = CLREnvironment();

            // a -> b
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:'0 -> b:'1):('0 -> '1)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var expected =
                Lambda(
                    BoundSymbol("a", ph0),
                    Identity("b", ph1),
                    Function(ph0, ph1));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation1()
        {
            var environment = CLREnvironment();

            // a:bool -> a
            var expression =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a"));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation2()
        {
            var environment = CLREnvironment();

            // a -> a:bool
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a", Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation3()
        {
            var environment = CLREnvironment();

            // a:bool -> a:bool
            var expression =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation4()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Type<bool>(),
                        Unspecified()));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation5()
        {
            var environment = CLREnvironment();

            // (a -> a):(_ -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Unspecified(),
                        Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation6()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Type<bool>(),
                        Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation7()
        {
            var environment = CLREnvironment();

            // (a -> a):(_ -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Unspecified(),
                        Unspecified()));

            var actual = environment.Infer(expression);

            // (a:'0 -> a:'0):('0 -> '0)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Lambda(
                    BoundSymbol("a", ph0),
                    Identity("a", ph0),
                    Function(ph0, ph0));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation8()
        {
            var environment = CLREnvironment();

            // (a -> b):(bool -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("b"),
                    Function(
                        Type<bool>(),
                        Unspecified()));

            var actual = environment.Infer(expression);

            // (a:bool -> b:'0):(bool -> '0)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b", ph0),
                    Function(Type<bool>(), ph0));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation9()
        {
            var environment = CLREnvironment();

            // (a -> b):(_ -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("b"),
                    Function(
                        Unspecified(),
                        Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:'0 -> b:bool):('0 -> bool)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Lambda(
                    BoundSymbol("a", ph0),
                    Identity("b", Type<bool>()),
                    Function(ph0, Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation10()
        {
            var environment = CLREnvironment();

            // (a -> b):(bool -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("b"),
                    Function(
                        Type<bool>(),
                        Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:bool -> b:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation11()
        {
            var environment = CLREnvironment();

            // (a -> b:bool):(bool -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("b", Type<bool>()),
                    Function(
                        Type<bool>(),
                        Unspecified()));

            var actual = environment.Infer(expression);

            // (a:bool -> b:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation12()
        {
            var environment = CLREnvironment();

            // (a:bool -> b):(_ -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b"),
                    Function(
                        Unspecified(),
                        Type<bool>()));

            var actual = environment.Infer(expression);

            // (a:bool -> b:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation13_1()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Identity("bool"),
                        Unspecified()));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool")),
                    Identity("a", Identity("bool")),
                    Function(
                        Identity("bool"),
                        Identity("bool")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation13_2()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> _):(* -> *)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Identity("bool"),
                        Unspecified(),
                        Function(
                            Kind(),
                            Kind())));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation14_1()
        {
            var environment = CLREnvironment();

            // (a -> a):(_ -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Unspecified(),
                        Identity("bool")));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool")),
                    Identity("a", Identity("bool")),
                    Function(
                        Identity("bool"),
                        Identity("bool")));

            var r = expected.GetPrettyString(PrettyStringTypes.Readable);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation14_2()
        {
            var environment = CLREnvironment();

            // (a -> a):(_ -> bool):(* -> *)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Unspecified(),
                        Identity("bool"),
                        Function(
                            Kind(),
                            Kind())));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            var r = expected.GetPrettyString(PrettyStringTypes.Readable);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation15_1()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> bool)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Identity("bool"),
                        Identity("bool")));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool")),
                    Identity("a", Identity("bool")),
                    Function(
                        Identity("bool"),
                        Identity("bool")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation15_2()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> bool):(* -> *)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Identity("bool"),
                        Identity("bool"),
                        Function(
                            Kind(),
                            Kind())));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation16()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> _):(* -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Identity("bool"),
                        Unspecified(),
                        Function(
                            Kind(),
                            Unspecified())));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation17()
        {
            var environment = CLREnvironment();

            // (a -> a):(_ -> bool):(_ -> *)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Unspecified(),
                        Identity("bool"),
                        Function(
                            Unspecified(),
                            Kind())));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation18()
        {
            var environment = CLREnvironment();

            // (a -> a):(_ -> bool):(* -> _)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Unspecified(),
                        Identity("bool"),
                        Function(
                            Kind(),
                            Unspecified())));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaWithAnnotation19()
        {
            var environment = CLREnvironment();

            // (a -> a):(bool -> _):(_ -> *)
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("a"),
                    Function(
                        Identity("bool"),
                        Unspecified(),
                        Function(
                            Unspecified(),
                            Kind())));

            var actual = environment.Infer(expression);

            // (a:bool:* -> a:bool:*):(bool:* -> bool:*):(* -> *)
            var expected =
                Lambda(
                    BoundSymbol("a", Identity("bool", Kind())),
                    Identity("a", Identity("bool", Kind())),
                    Function(
                        Identity("bool", Kind()),
                        Identity("bool", Kind()),
                        Function(
                            Kind(),
                            Kind())));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaShadowedVariable1()
        {
            var environment = CLREnvironment();

            // a = c:int
            environment.MutableBind(
                "a",
                Identity("c", Type<int>()));

            // a -> a:bool
            var expression =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a"));

            var actual = environment.Infer(expression);

            // (a:bool -> a:bool):(bool -> bool)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("a", Type<bool>()),
                    Function(Type<bool>(), Type<bool>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaShadowedVariable2()
        {
            var environment = CLREnvironment();

            // b:int = c
            environment.MutableBind(
                BoundSymbol("b", Type<int>()),
                Identity("c"));

            // a:bool -> b
            var expression =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:bool -> b:int):(bool -> int)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b", Type<int>()),
                    Function(Type<bool>(), Type<int>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaShadowedVariable3()
        {
            var environment = CLREnvironment();

            // b = 123:int
            environment.MutableBind(
                "b",
                Constant(123));

            // a:bool -> b
            var expression =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:bool -> b:int):(bool -> int)
            var expected =
                Lambda(
                    BoundSymbol("a", Type<bool>()),
                    Identity("b", Type<int>()),
                    Function(Type<bool>(), Type<int>()));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void LambdaComplex1()
        {
            var environment = CLREnvironment();

            // b = 123:int
            environment.MutableBind(
                "b",
                Constant(123));

            // a -> b
            var expression =
                Lambda(
                    BoundSymbol("a"),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:'0 -> b:int):('0 -> int)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Lambda(
                    BoundSymbol("a", ph0),
                    Identity("b", Type<int>()),
                    Function(ph0, Type<int>()));

            AssertLogicalEqual(expression, expected, actual);
        }
    }
}
