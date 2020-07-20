using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using NUnit.Framework;

using static Favalet.Generator;

namespace Favalet
{
    [TestFixture]
    public sealed class AlgebraicTest
    {
        private static readonly LogicalCalculator calculator =
            new LogicalCalculator();

        private static void AssertLogicalEqual(
            IExpression expression,
            IExpression expected,
            IExpression actual)
        {
            if (!calculator.Equals(expected, actual))
            {
                Assert.Fail(
                    "Expression = {0}\r\nExpected   = {1}\r\nActual     = {2}",
                    expression.GetPrettyString(PrettyStringTypes.Simple),
                    expected.GetPrettyString(PrettyStringTypes.Simple),
                    actual.GetPrettyString(PrettyStringTypes.Simple));
            }
        }

        #region And
        [Test]
        public void NonReduceSingleAnd()
        {
            var scope = Scope.Create();

            // A && B
            var expression =
                AndBinary(
                    Identity("A"),
                    Identity("B"));

            var actual = scope.Reduce(expression);

            // A && B
            var expected =
                AndBinary(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceSingleAnd()
        {
            var scope = Scope.Create();

            // A && B
            var expression =
                Logical(
                    AndBinary(
                        Identity("A"),
                        Identity("B")));

            var actual = scope.Reduce(expression);

            // A && B
            var expected =
                AndBinary(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NonReduceDuplicatedAnd()
        {
            var scope = Scope.Create();

            // (A && A) && A
            var expression =
                AndBinary(
                    AndBinary(
                        Identity("A"),
                        Identity("A")),
                    Identity("A"));

            var actual = scope.Reduce(expression);

            // (A && A) && A
            var expected =
                AndBinary(
                    AndBinary(
                        Identity("A"),
                        Identity("A")),
                    Identity("A"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceDuplicatedAnd()
        {
            var scope = Scope.Create();

            // (A && A) && A
            var expression =
                Logical(
                    AndBinary(
                        AndBinary(
                            Identity("A"),
                            Identity("A")),
                        Identity("A")));

            var actual = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceMultipleDuplicatedAnd()
        {
            var scope = Scope.Create();

            // (A && A) && (A && A)
            var expression =
                Logical(
                    AndBinary(
                        AndBinary(
                            Identity("A"),
                            Identity("A")),
                        AndBinary(
                            Identity("A"),
                            Identity("A"))));

            var actual = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion

        #region Or
        [Test]
        public void NonReduceSingleOr()
        {
            var scope = Scope.Create();

            // A || B
            var expression =
                OrBinary(
                    Identity("A"),
                    Identity("B"));

            var actual = scope.Reduce(expression);

            // A || B
            var expected =
                OrBinary(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceSingleOr()
        {
            var scope = Scope.Create();

            // A || B
            var expression =
                Logical(
                    OrBinary(
                        Identity("A"),
                        Identity("B")));

            var actual = scope.Reduce(expression);

            // A || B
            var expected =
                OrBinary(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NonReduceDuplicatedOr()
        {
            var scope = Scope.Create();

            // (A || A) || A
            var expression =
                OrBinary(
                    OrBinary(
                        Identity("A"),
                        Identity("A")),
                    Identity("A"));

            var actual = scope.Reduce(expression);

            // (A || A) || A
            var expected =
                OrBinary(
                    OrBinary(
                        Identity("A"),
                        Identity("A")),
                    Identity("A"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceDuplicatedOr()
        {
            var scope = Scope.Create();

            // (A || A) || A
            var expression =
                Logical(
                    OrBinary(
                        OrBinary(
                            Identity("A"),
                            Identity("A")),
                        Identity("A")));

            var actual = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceMultipleDuplicatedOr()
        {
            var scope = Scope.Create();

            // (A || A) || (A || A)
            var expression =
                Logical(
                    OrBinary(
                        OrBinary(
                            Identity("A"),
                            Identity("A")),
                        OrBinary(
                            Identity("A"),
                            Identity("A"))));

            var actual = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion

        #region CombinedAndOr
        [Test]
        public void ReduceDuplicatedCombinedAndOr()
        {
            var scope = Scope.Create();

            // (A || A) && (A || A)
            var expression =
                Logical(
                    AndBinary(
                        OrBinary(
                            Identity("A"),
                            Identity("A")),
                        OrBinary(
                            Identity("A"),
                            Identity("A"))));

            var actual = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceDuplicatedCombinedOrAnd()
        {
            var scope = Scope.Create();

            // (A && A) || (A && A)
            var expression =
                Logical(
                    OrBinary(
                        AndBinary(
                            Identity("A"),
                            Identity("A")),
                        AndBinary(
                            Identity("A"),
                            Identity("A"))));

            var actual = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialCombinedAndOr()
        {
            var scope = Scope.Create();

            // (A || A) && (B || B)
            var expression =
                Logical(
                    AndBinary(
                        OrBinary(
                            Identity("A"),
                            Identity("A")),
                        OrBinary(
                            Identity("B"),
                            Identity("B"))));

            var actual = scope.Reduce(expression);

            // A && B
            var expected =
                AndBinary(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialCombinedOrAnd()
        {
            var scope = Scope.Create();

            // (A && A) || (B && B)
            var expression =
                Logical(
                    OrBinary(
                        AndBinary(
                            Identity("A"),
                            Identity("A")),
                        AndBinary(
                            Identity("B"),
                            Identity("B"))));

            var actual = scope.Reduce(expression);

            // A || B
            var expected =
                OrBinary(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialDifferenceAndOr()
        {
            var scope = Scope.Create();

            // (A || B) && (A || B)
            var expression =
                Logical(
                    AndBinary(
                        OrBinary(
                            Identity("A"),
                            Identity("B")),
                        OrBinary(
                            Identity("A"),
                            Identity("B"))));

            var actual = scope.Reduce(expression);

            // A || B
            var expected =
                OrBinary(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialDifferenceOrAnd()
        {
            var scope = Scope.Create();

            // (A && B) || (A && B)
            var expression =
                Logical(
                    OrBinary(
                        AndBinary(
                            Identity("A"),
                            Identity("B")),
                        AndBinary(
                            Identity("A"),
                            Identity("B"))));

            var actual = scope.Reduce(expression);

            // A && B
            var expected =
                AndBinary(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialPartiallyAndOr()
        {
            var scope = Scope.Create();

            // Absorption

            // A && (A || B)
            var expression =
                Logical(
                    AndBinary(
                        Identity("A"),
                        OrBinary(
                            Identity("A"),
                            Identity("B"))));

            var actual = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialPartiallyOrAnd()
        {
            var scope = Scope.Create();

            // Absorption

            // A || (A && B)
            var expression =
                Logical(
                    OrBinary(
                        Identity("A"),
                        AndBinary(
                            Identity("A"),
                            Identity("B"))));

            var actual = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialAndOrTensor()
        {
            var scope = Scope.Create();

            // (A || B) && (B || A)
            var expression =
                Logical(
                    AndBinary(
                        OrBinary(
                            Identity("A"),
                            Identity("B")),
                        OrBinary(
                            Identity("B"),
                            Identity("A"))));

            var actual = scope.Reduce(expression);

            // A || B
            var expected =
                OrBinary(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialOrAndTensor()
        {
            var scope = Scope.Create();

            // (A && B) || (B && A)
            var expression =
                Logical(
                    OrBinary(
                        AndBinary(
                            Identity("A"),
                            Identity("B")),
                        AndBinary(
                            Identity("B"),
                            Identity("A"))));

            var actual = scope.Reduce(expression);

            var expected =
                AndBinary(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialAndOrMultipleTensorLogical1()
        {
            var scope = Scope.Create();

            // (A || (B || C)) && (B || (C || A))
            var expression =
                Logical(
                    AndBinary(
                        OrBinary(
                            Identity("A"),
                            OrBinary(
                                Identity("B"),
                                Identity("C"))),
                        OrBinary(
                            Identity("B"),
                            OrBinary(
                                Identity("C"),
                                Identity("A")))));

            var actual = scope.Reduce(expression);

            // A || B || C
            var expected =
                OrBinary(
                    Identity("A"),
                    OrBinary(
                        Identity("B"),
                        Identity("C")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialOrAndMultipleTensorLogical1()
        {
            var scope = Scope.Create();

            // (A && (B && C)) || (B && (C && A))
            var expression =
                Logical(
                    OrBinary(
                        AndBinary(
                            Identity("A"),
                            AndBinary(
                                Identity("B"),
                                Identity("C"))),
                        AndBinary(
                            Identity("B"),
                            AndBinary(
                                Identity("C"),
                                Identity("A")))));

            var actual = scope.Reduce(expression);

            // A && B && C
            var expected =
                AndBinary(
                    Identity("A"),
                    AndBinary(
                        Identity("B"),
                        Identity("C")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialAndOrMultipleTensorLogical2()
        {
            var scope = Scope.Create();

            // (A || (B || C)) && ((C || A) || B)
            var expression =
                Logical(
                    AndBinary(
                        OrBinary(
                            Identity("A"),
                            OrBinary(
                                Identity("B"),
                                Identity("C"))),
                        OrBinary(
                            OrBinary(
                                Identity("C"),
                                Identity("A")),
                            Identity("B"))));

            var actual = scope.Reduce(expression);

            // A || B || C
            var expected =
                OrBinary(
                    Identity("A"),
                    OrBinary(
                        Identity("B"),
                        Identity("C")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialOrAndMultipleTensorLogical2()
        {
            var scope = Scope.Create();

            // (A && (B && C)) || ((C && A) && B)
            var expression =
                Logical(
                    OrBinary(
                        AndBinary(
                            Identity("A"),
                            AndBinary(
                                Identity("B"),
                                Identity("C"))),
                        AndBinary(
                            AndBinary(
                                Identity("C"),
                                Identity("A")),
                            Identity("B"))));

            var actual = scope.Reduce(expression);

            // A && B && C
            var expected =
                AndBinary(
                    Identity("A"),
                    AndBinary(
                        Identity("B"),
                        Identity("C")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialAndOrMultipleTensorLogical3()
        {
            var scope = Scope.Create();

            // ((A || B) || C) && (B || (C || A))
            var expression =
                Logical(
                    AndBinary(
                        OrBinary(
                            OrBinary(
                                Identity("A"),
                                Identity("B")),
                            Identity("C")),
                        OrBinary(
                            Identity("B"),
                            OrBinary(
                                Identity("C"),
                                Identity("A")))));

            var actual = scope.Reduce(expression);

            // A || B || C
            var expected =
                OrBinary(
                    Identity("A"),
                    OrBinary(
                        Identity("B"),
                        Identity("C")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialOrAndMultipleTensorLogical3()
        {
            var scope = Scope.Create();

            // ((A && B) && C) || (B && (C && A))
            var expression =
                Logical(
                    OrBinary(
                        AndBinary(
                            AndBinary(
                                Identity("A"),
                                Identity("B")),
                            Identity("C")),
                        AndBinary(
                            Identity("B"),
                            AndBinary(
                                Identity("C"),
                                Identity("A")))));

            var actual = scope.Reduce(expression);

            // A && B && C
            var expected =
                AndBinary(
                    Identity("A"),
                    AndBinary(
                        Identity("B"),
                        Identity("C")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialAndOrComplex()
        {
            var scope = Scope.Create();

            // Absorption

            // (A && (A || B)) || ((C && A) && B)
            var expression =
                Logical(
                    OrBinary(
                        AndBinary(
                            Identity("A"),
                            OrBinary(
                                Identity("A"),
                                Identity("B"))),
                        AndBinary(
                            AndBinary(
                                Identity("C"),
                                Identity("A")),
                            Identity("B"))));

            var actual = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialOrAndComplex()
        {
            var scope = Scope.Create();

            // Absorption

            // (A || (A && B)) && ((C || A) || B)
            var expression =
                Logical(
                    AndBinary(
                        OrBinary(
                            Identity("A"),
                            AndBinary(
                                Identity("A"),
                                Identity("B"))),
                        OrBinary(
                            OrBinary(
                                Identity("C"),
                                Identity("A")),
                            Identity("B"))));

            var actual = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion
    }
}
