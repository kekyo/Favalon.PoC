using Favalet.Contexts;
using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using NUnit.Framework;

using static Favalet.Generator;

namespace Favalet.Reducing
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
            if (!calculator.ExactEquals(expected, actual))
            {
                Assert.Fail(
                    "Expression = {0}\r\nExpected   = {1}\r\nActual     = {2}",
                    expression.GetPrettyString(PrettyStringTypes.Readable),
                    expected.GetPrettyString(PrettyStringTypes.Readable),
                    actual.GetPrettyString(PrettyStringTypes.Readable));
            }
        }

        #region And
        [Test]
        public void NonReduceSingleAnd()
        {
            var environment = Environment();

            // A && B
            var expression =
                And(
                    Identity("A"),
                    Identity("B"));

            var actual = environment.Reduce(expression);

            // A && B
            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceSingleAnd()
        {
            var environment = Environment();

            // A && B
            var expression =
                Logical(
                    And(
                        Identity("A"),
                        Identity("B")));

            var actual = environment.Reduce(expression);

            // A && B
            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NonReduceDuplicatedAnd()
        {
            var environment = Environment();

            // (A && A) && A
            var expression =
                And(
                    And(
                        Identity("A"),
                        Identity("A")),
                    Identity("A"));

            var actual = environment.Reduce(expression);

            // (A && A) && A
            var expected =
                And(
                    And(
                        Identity("A"),
                        Identity("A")),
                    Identity("A"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceDuplicatedAnd()
        {
            var environment = Environment();

            // (A && A) && A
            var expression =
                Logical(
                    And(
                        And(
                            Identity("A"),
                            Identity("A")),
                        Identity("A")));

            var actual = environment.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceMultipleDuplicatedAnd()
        {
            var environment = Environment();

            // (A && A) && (A && A)
            var expression =
                Logical(
                    And(
                        And(
                            Identity("A"),
                            Identity("A")),
                        And(
                            Identity("A"),
                            Identity("A"))));

            var actual = environment.Reduce(expression);

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
            var environment = Environment();

            // A || B
            var expression =
                Or(
                    Identity("A"),
                    Identity("B"));

            var actual = environment.Reduce(expression);

            // A || B
            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceSingleOr()
        {
            var environment = Environment();

            // A || B
            var expression =
                Logical(
                    Or(
                        Identity("A"),
                        Identity("B")));

            var actual = environment.Reduce(expression);

            // A || B
            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void NonReduceDuplicatedOr()
        {
            var environment = Environment();

            // (A || A) || A
            var expression =
                Or(
                    Or(
                        Identity("A"),
                        Identity("A")),
                    Identity("A"));

            var actual = environment.Reduce(expression);

            // (A || A) || A
            var expected =
                Or(
                    Or(
                        Identity("A"),
                        Identity("A")),
                    Identity("A"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceDuplicatedOr()
        {
            var environment = Environment();

            // (A || A) || A
            var expression =
                Logical(
                    Or(
                        Or(
                            Identity("A"),
                            Identity("A")),
                        Identity("A")));

            var actual = environment.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceMultipleDuplicatedOr()
        {
            var environment = Environment();

            // (A || A) || (A || A)
            var expression =
                Logical(
                    Or(
                        Or(
                            Identity("A"),
                            Identity("A")),
                        Or(
                            Identity("A"),
                            Identity("A"))));

            var actual = environment.Reduce(expression);

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
            var environment = Environment();

            // (A || A) && (A || A)
            var expression =
                Logical(
                    And(
                        Or(
                            Identity("A"),
                            Identity("A")),
                        Or(
                            Identity("A"),
                            Identity("A"))));

            var actual = environment.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReduceDuplicatedCombinedOrAnd()
        {
            var environment = Environment();

            // (A && A) || (A && A)
            var expression =
                Logical(
                    Or(
                        And(
                            Identity("A"),
                            Identity("A")),
                        And(
                            Identity("A"),
                            Identity("A"))));

            var actual = environment.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialCombinedAndOr()
        {
            var environment = Environment();

            // (A || A) && (B || B)
            var expression =
                Logical(
                    And(
                        Or(
                            Identity("A"),
                            Identity("A")),
                        Or(
                            Identity("B"),
                            Identity("B"))));

            var actual = environment.Reduce(expression);

            // A && B
            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialCombinedOrAnd()
        {
            var environment = Environment();

            // (A && A) || (B && B)
            var expression =
                Logical(
                    Or(
                        And(
                            Identity("A"),
                            Identity("A")),
                        And(
                            Identity("B"),
                            Identity("B"))));

            var actual = environment.Reduce(expression);

            // A || B
            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialDifferenceAndOr()
        {
            var environment = Environment();

            // (A || B) && (A || B)
            var expression =
                Logical(
                    And(
                        Or(
                            Identity("A"),
                            Identity("B")),
                        Or(
                            Identity("A"),
                            Identity("B"))));

            var actual = environment.Reduce(expression);

            // A || B
            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialDifferenceOrAnd()
        {
            var environment = Environment();

            // (A && B) || (A && B)
            var expression =
                Logical(
                    Or(
                        And(
                            Identity("A"),
                            Identity("B")),
                        And(
                            Identity("A"),
                            Identity("B"))));

            var actual = environment.Reduce(expression);

            // A && B
            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialPartiallyAndOr()
        {
            var environment = Environment();

            // Absorption

            // A && (A || B)
            var expression =
                Logical(
                    And(
                        Identity("A"),
                        Or(
                            Identity("A"),
                            Identity("B"))));

            var actual = environment.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialPartiallyOrAnd()
        {
            var environment = Environment();

            // Absorption

            // A || (A && B)
            var expression =
                Logical(
                    Or(
                        Identity("A"),
                        And(
                            Identity("A"),
                            Identity("B"))));

            var actual = environment.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialAndOrTensor()
        {
            var environment = Environment();

            // (A || B) && (B || A)
            var expression =
                Logical(
                    And(
                        Or(
                            Identity("A"),
                            Identity("B")),
                        Or(
                            Identity("B"),
                            Identity("A"))));

            var actual = environment.Reduce(expression);

            // A || B
            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialOrAndTensor()
        {
            var environment = Environment();

            // (A && B) || (B && A)
            var expression =
                Logical(
                    Or(
                        And(
                            Identity("A"),
                            Identity("B")),
                        And(
                            Identity("B"),
                            Identity("A"))));

            var actual = environment.Reduce(expression);

            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialAndOrMultipleTensorLogical1()
        {
            var environment = Environment();

            // (A || (B || C)) && (B || (C || A))
            var expression =
                Logical(
                    And(
                        Or(
                            Identity("A"),
                            Or(
                                Identity("B"),
                                Identity("C"))),
                        Or(
                            Identity("B"),
                            Or(
                                Identity("C"),
                                Identity("A")))));

            var actual = environment.Reduce(expression);

            // A || B || C
            var expected =
                Or(
                    Identity("A"),
                    Or(
                        Identity("B"),
                        Identity("C")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialOrAndMultipleTensorLogical1()
        {
            var environment = Environment();

            // (A && (B && C)) || (B && (C && A))
            var expression =
                Logical(
                    Or(
                        And(
                            Identity("A"),
                            And(
                                Identity("B"),
                                Identity("C"))),
                        And(
                            Identity("B"),
                            And(
                                Identity("C"),
                                Identity("A")))));

            var actual = environment.Reduce(expression);

            // A && B && C
            var expected =
                And(
                    Identity("A"),
                    And(
                        Identity("B"),
                        Identity("C")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialAndOrMultipleTensorLogical2()
        {
            var environment = Environment();

            // (A || (B || C)) && ((C || A) || B)
            var expression =
                Logical(
                    And(
                        Or(
                            Identity("A"),
                            Or(
                                Identity("B"),
                                Identity("C"))),
                        Or(
                            Or(
                                Identity("C"),
                                Identity("A")),
                            Identity("B"))));

            var actual = environment.Reduce(expression);

            // A || B || C
            var expected =
                Or(
                    Identity("A"),
                    Or(
                        Identity("B"),
                        Identity("C")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialOrAndMultipleTensorLogical2()
        {
            var environment = Environment();

            // (A && (B && C)) || ((C && A) && B)
            var expression =
                Logical(
                    Or(
                        And(
                            Identity("A"),
                            And(
                                Identity("B"),
                                Identity("C"))),
                        And(
                            And(
                                Identity("C"),
                                Identity("A")),
                            Identity("B"))));

            var actual = environment.Reduce(expression);

            // A && B && C
            var expected =
                And(
                    Identity("A"),
                    And(
                        Identity("B"),
                        Identity("C")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialAndOrMultipleTensorLogical3()
        {
            var environment = Environment();

            // ((A || B) || C) && (B || (C || A))
            var expression =
                Logical(
                    And(
                        Or(
                            Or(
                                Identity("A"),
                                Identity("B")),
                            Identity("C")),
                        Or(
                            Identity("B"),
                            Or(
                                Identity("C"),
                                Identity("A")))));

            var actual = environment.Reduce(expression);

            // A || B || C
            var expected =
                Or(
                    Identity("A"),
                    Or(
                        Identity("B"),
                        Identity("C")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialOrAndMultipleTensorLogical3()
        {
            var environment = Environment();

            // ((A && B) && C) || (B && (C && A))
            var expression =
                Logical(
                    Or(
                        And(
                            And(
                                Identity("A"),
                                Identity("B")),
                            Identity("C")),
                        And(
                            Identity("B"),
                            And(
                                Identity("C"),
                                Identity("A")))));

            var actual = environment.Reduce(expression);

            // A && B && C
            var expected =
                And(
                    Identity("A"),
                    And(
                        Identity("B"),
                        Identity("C")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialAndOrComplex()
        {
            var environment = Environment();

            // Absorption

            // (A && (A || B)) || ((C && A) && B)
            var expression =
                Logical(
                    Or(
                        And(
                            Identity("A"),
                            Or(
                                Identity("A"),
                                Identity("B"))),
                        And(
                            And(
                                Identity("C"),
                                Identity("A")),
                            Identity("B"))));

            var actual = environment.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ReducePartialOrAndComplex()
        {
            var environment = Environment();

            // Absorption

            // (A || (A && B)) && ((C || A) || B)
            var expression =
                Logical(
                    And(
                        Or(
                            Identity("A"),
                            And(
                                Identity("A"),
                                Identity("B"))),
                        Or(
                            Or(
                                Identity("C"),
                                Identity("A")),
                            Identity("B"))));

            var actual = environment.Reduce(expression);

            // A
            var expected =
                Identity("A");

            AssertLogicalEqual(expression, expected, actual);
        }
        #endregion
    }
}
