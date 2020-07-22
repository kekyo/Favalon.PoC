using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using NUnit.Framework;
using System;
using System.Collections;

using static Favalet.CLRGenerator;
using static Favalet.Generator;

namespace Favalet
{
    [TestFixture]
    public sealed class ApplicableTest
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

        [Test]
        public void LookupIdentity()
        {
            var scope = Scope.Create();

            scope.SetVariable("ABC", Identity("XYZ"));

            var expression =
                Identity("ABC");

            var actual = scope.Reduce(expression);

            var expected =
                Identity("XYZ");

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void PureLambda()
        {
            var scope = Scope.Create();

            var expression =
                Lambda(
                    "arg",
                    And(
                        Identity("arg"),
                        Identity("B")));

            var actual = scope.Reduce(expression);

            var expected =
                Lambda(
                    "arg",
                    And(
                        Identity("arg"),
                        Identity("B")));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyLambda1()
        {
            var scope = Scope.Create();

            // (arg -> arg && B) A
            var expression =
                Apply(
                    Lambda(
                        "arg",
                        And(
                            Identity("arg"),
                            Identity("B"))),
                    Identity("A"));

            var actual = scope.Reduce(expression);

            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyLambda2()
        {
            var scope = Scope.Create();

            // inner = arg1 -> arg1 && C
            scope.SetVariable(
                "inner",
                Lambda(
                    "arg1",
                    And(
                        Identity("arg1"),
                        Identity("B"))));

            // (arg2 -> inner arg2) A
            var expression =
                Apply(
                    Lambda(
                        "arg2",
                        Apply(
                            Identity("inner"),
                            Identity("arg2"))),
                    Identity("A"));

            var actual = scope.Reduce(expression);

            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyLambda3()
        {
            var scope = Scope.Create();

            // Same argument symbols.

            // inner = arg -> arg && C
            scope.SetVariable(
                "inner",
                Lambda(
                    "arg",
                    And(
                        Identity("arg"),
                        Identity("B"))));

            // (arg -> inner arg) A
            var expression =
                Apply(
                    Lambda(
                        "arg",
                        Apply(
                            Identity("inner"),
                            Identity("arg"))),
                    Identity("A"));

            var actual = scope.Reduce(expression);

            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyNestedLambda()
        {
            var scope = Scope.Create();

            // Complex nested lambda (bind)

            // inner = arg1 -> arg2 -> arg2 && arg1
            scope.SetVariable(
                "inner",
                Lambda(
                    "arg1",
                    Lambda(
                        "arg2",
                        And(
                            Identity("arg2"),
                            Identity("arg1")))));

            // inner A B
            var expression =
                Apply(
                    Apply(
                        Identity("inner"),
                        Identity("A")),
                    Identity("B"));

            var actual = scope.Reduce(expression);

            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyLogicalOperator1()
        {
            var scope = Scope.Create();

            // Logical (B && A)
            var expression =
                Apply(
                    Logical(),
                    And(
                        Identity("B"),
                        Identity("A")));

            var actual = scope.Reduce(expression);

            // A && B
            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyLogicalOperator2()
        {
            var scope = Scope.Create();

            // logical = Logical
            scope.SetVariable(
                "logical",
                Logical());

            // logical (B && A)
            var expression =
                Apply(
                    Identity("logical"),
                    And(
                        Identity("B"),
                        Identity("A")));

            var actual = scope.Reduce(expression);

            // A && B
            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            AssertLogicalEqual(expression, expected, actual);
        }
    }
}
