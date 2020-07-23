﻿using Favalet.Contexts;
using Favalet.Expressions;
using Favalet.Expressions.Algebraic;
using NUnit.Framework;
using System;

using static Favalet.Generator;
using static Favalet.CLRGenerator;

namespace Favalet.Reduces
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
            if (!calculator.ExactEquals(expected, actual))
            {
                Assert.Fail(
                    "Expression = {0}\r\nExpected   = {1}\r\nActual     = {2}",
                    expression.GetPrettyString(PrettyStringContext.Simple),
                    expected.GetPrettyString(PrettyStringContext.Simple),
                    actual.GetPrettyString(PrettyStringContext.Simple));
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

        [Test]
        public void ApplyMethod()
        {
            var scope = Scope.Create();

            // Math.Sqrt pi
            var expression =
                Apply(
                    Method(typeof(Math).GetMethod("Sqrt")!),
                    Constant(Math.PI));

            var actual = scope.Reduce(expression);

            // A && B
            var expected =
                Constant(Math.Sqrt(Math.PI));

            AssertLogicalEqual(expression, expected, actual);
        }

        [Test]
        public void ApplyConstructor()
        {
            var scope = Scope.Create();

            // Uri "http://example.com"
            var expression =
                Apply(
                    Method(typeof(Uri).GetConstructor(new[] { typeof(string) })!),
                    Constant("http://example.com"));

            var actual = scope.Reduce(expression);

            // Uri
            var expected =
                Constant(new Uri("http://example.com"));

            AssertLogicalEqual(expression, expected, actual);
        }
    }
}
