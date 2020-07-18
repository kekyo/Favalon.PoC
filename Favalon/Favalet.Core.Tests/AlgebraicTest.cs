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
        public void ReduceSingleAnd()
        {
            var scope = Scope.Create();

            var expression =
                AndBinary(
                    Identity("A"),
                    Identity("B"));

            var reduced = scope.Reduce(expression);

            Assert.AreEqual(expression, reduced);
        }

        [Test]
        public void ReduceSingleAndLogical()
        {
            var scope = Scope.Create();

            var expression =
                Logical(
                    AndBinary(
                        Identity("A"),
                        Identity("B")));

            var reduced = scope.Reduce(expression);

            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReduceNonReducibleSingleAnd()
        {
            var scope = Scope.Create();

            var expression =
                AndBinary(
                    Identity("A"),
                    Identity("A"),
                    Identity("A"));

            var reduced = scope.Reduce(expression);

            Assert.AreEqual(expression, reduced);
        }

        [Test]
        public void ReduceReducibleSingleAndLogical()
        {
            var scope = Scope.Create();

            var expression =
                Logical(
                    AndBinary(
                        Identity("A"),
                        Identity("A"),
                        Identity("A")));

            var reduced = scope.Reduce(expression);

            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReduceReducibleMultipleAndLogical()
        {
            var scope = Scope.Create();

            var expression =
                Logical(
                    AndBinary(
                        Identity("A"),
                        Identity("A"),
                        AndBinary(
                            Identity("A"),
                            Identity("A"))));

            var reduced = scope.Reduce(expression);

            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
        }
        #endregion

        #region Or
        [Test]
        public void ReduceSingleOr()
        {
            var scope = Scope.Create();

            var expression =
                OrBinary(
                    Identity("A"),
                    Identity("B"));

            var reduced = scope.Reduce(expression);

            Assert.AreEqual(expression, reduced);
        }

        [Test]
        public void ReduceSingleOrLogical()
        {
            var scope = Scope.Create();

            var expression =
                Logical(
                    OrBinary(
                        Identity("A"),
                        Identity("B")));

            var reduced = scope.Reduce(expression);

            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReduceNonReducibleSingleOr()
        {
            var scope = Scope.Create();

            var expression =
                OrBinary(
                    Identity("A"),
                    Identity("A"),
                    Identity("A"));

            var reduced = scope.Reduce(expression);

            Assert.AreEqual(expression, reduced);
        }

        [Test]
        public void ReduceReducibleSingleOrLogical()
        {
            var scope = Scope.Create();

            var expression =
                Logical(
                    OrBinary(
                        Identity("A"),
                        Identity("A"),
                        Identity("A")));

            var reduced = scope.Reduce(expression);

            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReduceReducibleMultipleOrLogical()
        {
            var scope = Scope.Create();

            var expression =
                Logical(
                    OrBinary(
                        Identity("A"),
                        Identity("A"),
                        OrBinary(
                            Identity("A"),
                            Identity("A"))));

            var reduced = scope.Reduce(expression);

            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
        }
        #endregion

        #region CombinedAndOr
        [Test]
        public void ReduceReducibleCombinedAndOrLogical()
        {
            var scope = Scope.Create();

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

            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReduceReducibleCombinedOrAndLogical()
        {
            var scope = Scope.Create();

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

            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReducePartialReducibleCombinedAndOrLogical()
        {
            var scope = Scope.Create();

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

            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReducePartialReducibleCombinedOrAndLogical()
        {
            var scope = Scope.Create();

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

            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReducePartialReducibleDifferenceAndOrLogical()
        {
            var scope = Scope.Create();

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

            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReducePartialReducibleDifferenceOrAndLogical()
        {
            var scope = Scope.Create();

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

            var expected =
                And(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReducePartialReduciblePartiallyAndOrLogical()
        {
            var scope = Scope.Create();

            // Absorption
            var expression =
                Logical(
                    AndBinary(
                        Identity("A"),
                        OrBinary(
                            Identity("A"),
                            Identity("B"))));

            var reduced = scope.Reduce(expression);

            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReducePartialReduciblePartiallyOrAndLogical()
        {
            var scope = Scope.Create();

            // Absorption
            var expression =
                Logical(
                    OrBinary(
                        Identity("A"),
                        AndBinary(
                            Identity("A"),
                            Identity("B"))));

            var reduced = scope.Reduce(expression);

            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReducePartialReducibleAndOrTensorLogical()
        {
            var scope = Scope.Create();

            // Absorption
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

            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReducePartialReducibleOrAndTensorLogical()
        {
            var scope = Scope.Create();

            // Absorption
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
        public void ReducePartialReducibleAndOrMultipleTensorLogical()
        {
            var scope = Scope.Create();

            // Absorption

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
        #endregion
    }
}
