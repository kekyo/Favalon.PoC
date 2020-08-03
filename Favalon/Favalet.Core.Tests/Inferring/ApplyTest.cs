using Favalet.Contexts;
using Favalet.Expressions;
using NUnit.Framework;
using System;

using static Favalet.CLRGenerator;
using static Favalet.Generator;

namespace Favalet.Inferring
{
    [TestFixture]
    public sealed class ApplyTest
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
        public void ApplyWithoutAnnotation()
        {
            var environment = CLREnvironment();

            // a b
            var expression =
                Apply(
                    Identity("a"),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:('0 -> '1) b:'0):'1
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Identity("a", Function(ph0, ph1)),
                    Identity("b", ph0),
                    ph1);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyNestedFunctionWithoutAnnotation1()
        {
            var environment = CLREnvironment();

            // a a
            var expression =
                Apply(
                    Identity("a"),
                    Identity("a"));

            var actual = environment.Infer(expression);

            // (a:('0 -> '1) a:'0):'1
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Identity("a", Function(ph0, ph1)),
                    Identity("a", ph0),
                    ph1);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyNestedFunctionWithoutAnnotation2()
        {
            var environment = CLREnvironment();

            // a = x -> x
            environment.MutableBind(
                "a",
                Lambda(
                    "x",
                    Identity("x")));

            // a a
            var expression =
                Apply(
                    Identity("a"),
                    Identity("a"));

            var actual = environment.Infer(expression);

            // ((x:('0 -> '0) -> x:('0 -> '0)) (x:'0 -> x:'0)):('0 -> '0)
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Identity(
                        "a",
                        Function(
                            Function(ph0, ph0),
                            Function(ph0, ph0))),
                    Identity(
                        "a",
                        Function(ph0, ph0)),
                    Function(
                        ph0,
                        ph0));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyWithAnnotation1()
        {
            var environment = CLREnvironment();

            // a:(bool -> _) b
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Unspecified())),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:(bool -> '0) b:bool):'0
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Identity("a",
                        Function(Identity("bool"), ph0)),
                    Identity("b", Identity("bool")),
                    ph0);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyWithAnnotation2()
        {
            var environment = CLREnvironment();

            // a:(_ -> bool) b
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Unspecified(),
                            Identity("bool"))),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:('0 -> bool) b:'0):bool
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Identity("a",
                        Function(ph0, Identity("bool"))),
                    Identity("b", ph0),
                    Identity("bool"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyWithAnnotation3()
        {
            var environment = CLREnvironment();

            // a:(int -> bool) b
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Identity("int"),
                            Identity("bool"))),
                    Identity("b"));

            var actual = environment.Infer(expression);

            // (a:(int -> bool) b:int):bool
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("int"),
                            Identity("bool"))),
                    Identity("b", Identity("int")),
                    Identity("bool"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyWithAnnotation4()
        {
            var environment = CLREnvironment();

            // a b:bool
            var expression =
                Apply(
                    Identity("a"),
                    Identity("b", Identity("bool")));

            var actual = environment.Infer(expression);

            // (a:(bool -> _) b:bool):_
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Unspecified())),
                    Identity("b", Identity("bool")),
                    Unspecified());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyWithAnnotation5()
        {
            var environment = CLREnvironment();

            // (a b):bool
            var expression =
                Apply(
                    Identity("a"),
                    Identity("b"),
                    Identity("bool"));

            var actual = environment.Infer(expression);

            // (a:('0 -> bool) b:'0):bool
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            ph0,
                            Identity("bool"))),
                    Identity("b", ph0),
                    Identity("bool"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyWithAnnotation6()
        {
            var environment = CLREnvironment();

            // (a b:bool):int
            var expression =
                Apply(
                    Identity("a"),
                    Identity("b", Identity("bool")),
                    Identity("int"));

            var actual = environment.Infer(expression);

            // (a:(bool -> int) b:bool):int
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Identity("int"))),
                    Identity("b", Identity("bool")),
                    Identity("int"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyWithAnnotation7()
        {
            var environment = CLREnvironment();

            // a:(_ -> int) b:bool
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Unspecified(),
                            Identity("int"))),
                    Identity("b", Identity("bool")));

            var actual = environment.Infer(expression);

            // (a:(bool -> int) b:bool):int
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Unspecified())),
                    Identity("b", Identity("bool")),
                    Unspecified());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyWithAnnotation8()
        {
            var environment = CLREnvironment();

            // a:(_ -> int) b:bool
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Unspecified(),
                            Identity("int"))),
                    Identity("b", Identity("bool")));

            var actual = environment.Infer(expression);

            // (a:(bool -> int) b:bool):int
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Unspecified())),
                    Identity("b", Identity("bool")),
                    Unspecified());

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyWithAnnotation9()
        {
            var environment = CLREnvironment();

            // (a:(bool -> _) b):int
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Unspecified())),
                    Identity("b"),
                    Identity("int"));

            var actual = environment.Infer(expression);

            // (a:(bool -> int) b:bool):int
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Identity("int"))),
                    Identity("b", Identity("bool")),
                    Identity("int"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyWithAnnotation10()
        {
            var environment = CLREnvironment();

            // (a:(bool -> int) b:bool):int
            var expression =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Identity("int"))),
                    Identity("b", Identity("bool")),
                    Identity("int"));

            var actual = environment.Infer(expression);

            // (a:(bool -> int) b:bool):int
            var expected =
                Apply(
                    Identity("a",
                        Function(
                            Identity("bool"),
                            Identity("int"))),
                    Identity("b", Identity("bool")),
                    Identity("int"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NestedApplyWithoutAnnotation()
        {
            var environment = CLREnvironment();

            // a b c
            var expression =
                Apply(
                    Apply(
                        Identity("a"),
                        Identity("b")),
                    Identity("c"));

            var actual = environment.Infer(expression);

            // ((a:('0 -> ('1 -> '2)) b:'0 c:'1):'2
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var ph2 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Apply(
                        Identity("a",
                            Function(
                                ph0,
                                Function(ph1, ph2))),
                        Identity("b", ph0)),
                    Identity("c", ph1),
                    ph2);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NestedApplyWithAnnotation1()
        {
            var environment = CLREnvironment();

            // a b c:bool
            var expression =
                Apply(
                    Apply(
                        Identity("a"),
                        Identity("b")),
                    Identity("c", Identity("bool")));

            var actual = environment.Infer(expression);

            // ((a:('0 -> (bool -> '1)) b:'0 c:bool):'1
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Apply(
                        Identity("a",
                            Function(
                                ph0,
                                Function(Identity("bool"), ph1))),
                        Identity("b", ph0)),
                    Identity("c", Identity("bool")),
                    ph1);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NestedApplyWithAnnotation2()
        {
            var environment = CLREnvironment();

            // a b:bool c
            var expression =
                Apply(
                    Apply(
                        Identity("a"),
                        Identity("b", Identity("bool"))),
                    Identity("c"));

            var actual = environment.Infer(expression);

            // ((a:(bool -> ('0 -> '1)) b:bool c:'0):'1
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Apply(
                        Identity("a",
                            Function(
                                Identity("bool"),
                                Function(ph0, ph1))),
                        Identity("b", Identity("bool"))),
                    Identity("c", ph0),
                    ph1);

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NestedApplyWithAnnotation3()
        {
            var environment = CLREnvironment();

            // a:(bool -> _) b c
            var expression =
                Apply(
                    Apply(
                        Identity("a",
                            Function(Identity("bool"), Unspecified())),
                        Identity("b")),
                    Identity("c"));

            var actual = environment.Infer(expression);

            // ((a:(bool -> ('0 -> '1)) b:bool c:'0):'1
            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var ph1 = provider.CreatePlaceholder();
            var expected =
                Apply(
                    Apply(
                        Identity("a",
                            Function(
                                Identity("bool"),
                                Function(ph0, ph1))),
                        Identity("b", Identity("bool"))),
                    Identity("c", ph0),
                    ph1);

            AssertLogicalEqual(expression, expected, actual);
        }

        //[Test]
        public void ApplyYCombinator1()
        {
            var environment = CLREnvironment();

            // Y = f -> f (Y f)
            environment.MutableBind(
                "Y",
                Lambda(
                    "f",
                    Apply(
                        Identity("f"),
                        Apply(
                            Identity("Y"),
                            Identity("f")))));

            // Y
            var expression =
                Identity("Y");

            var actual = environment.Infer(expression);

            var expected =
                Identity("TODO:");

            AssertLogicalEqual(expression, expected, actual);
        }

        //[Test]
        public void ApplyYCombinator2()
        {
            var environment = CLREnvironment();

            // Y = f -> (x -> f (x x)) (x -> f (x x))
            var expression =
                Lambda(
                    "f",
                    Apply(
                        Lambda( // x -> f (x x)
                            "x",
                            Apply(
                                Identity("f"),
                                Apply(
                                    Identity("x"),
                                    Identity("x")))),
                        Lambda( // x -> f (x x)
                            "x",
                            Apply(
                                Identity("f"),
                                Apply(
                                    Identity("x"),
                                    Identity("x"))))));

            var actual = environment.Infer(expression);

            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Identity("TODO:");

            AssertLogicalEqual(expression, expected, actual);
        }

        //[Test]
        public void ApplyZCombinator()
        {
            var environment = CLREnvironment();

            // Z = f -> (x -> f (y -> x x y)) (x -> f (y -> x x y))
            var expression =
                Lambda(
                    "f",
                    Apply(
                        Lambda( // x -> f (y -> x x y)
                            "x",
                            Apply(
                                Identity("f"),
                                Lambda( // y -> x x y
                                    "y",
                                    Apply(
                                        Apply(
                                            Identity("x"),
                                            Identity("x")),
                                        Identity("y"))))),
                        Lambda( // x -> f (y -> x x y)
                            "x",
                            Apply(
                                Identity("f"),
                                Lambda( // y -> x x y
                                    "y",
                                    Apply(
                                        Apply(
                                            Identity("x"),
                                            Identity("x")),
                                        Identity("y")))))));

            var actual = environment.Infer(expression);

            var provider = PseudoPlaceholderProvider.Create();
            var ph0 = provider.CreatePlaceholder();
            var expected =
                Identity("TODO:");

            AssertLogicalEqual(expression, expected, actual);
        }


        // TODO: 異なる型でInferする場合をテストする
        //    1: 代入互換性が無いものはその場でエラー？ 又はASTを変形しないで返すとか？
        //    2: 代入互換性があるもの
        //    2-1: 正方向？（例えば引数）
        //    2-2: 逆方向？（例えば戻り値）


        // TODO: Unmatched orders
    }
}
