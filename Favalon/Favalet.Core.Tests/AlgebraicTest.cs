using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Favalet.Generator;

namespace Favalet
{
    [TestFixture]
    public sealed class AlgebraicTest
    {
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

            var reduced = scope.Reduce(expression);

            Assert.AreEqual(expression, reduced);
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

            var reduced = scope.Reduce(expression);

            // A && B
            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            Assert.AreEqual(expression, reduced);
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

            var reduced = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            Assert.AreEqual(expression, reduced);
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

            var reduced = scope.Reduce(expression);

            // A || B
            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            Assert.AreEqual(expression, reduced);
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

            var reduced = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A && B
            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A || B
            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A || B
            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A && B
            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A || B
            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A || B || C
            var expected =
                Or(
                    Identity("A"),
                    Identity("B"),
                    Identity("C"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A && B && C
            var expected =
                And(
                    Identity("A"),
                    Identity("B"),
                    Identity("C"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A || B || C
            var expected =
                Or(
                    Identity("A"),
                    Identity("B"),
                    Identity("C"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A && B && C
            var expected =
                And(
                    Identity("A"),
                    Identity("B"),
                    Identity("C"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A || B || C
            var expected =
                Or(
                    Identity("A"),
                    Identity("B"),
                    Identity("C"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A && B && C
            var expected =
                And(
                    Identity("A"),
                    Identity("B"),
                    Identity("C"));

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
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

            var reduced = scope.Reduce(expression);

            // A
            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
        }
        #endregion
    }
}
