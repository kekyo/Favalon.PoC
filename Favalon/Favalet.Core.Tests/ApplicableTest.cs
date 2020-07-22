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

        //[Test]
        //public void ApplyLambda()
        //{
        //    var scope = Scope.Create();

        //    var expression =
        //        Apply(
        //            Lambda(
        //                "arg",
        //                And(
        //                    Identity("arg"),
        //                    Identity("B"))),
        //            Identity("A"));

        //    var actual = scope.Reduce(expression);

        //    var expected =
        //        And(
        //            Identity("A"),
        //            Identity("B"));

        //    AssertLogicalEqual(expression, expected, actual);
        //}
    }
}
