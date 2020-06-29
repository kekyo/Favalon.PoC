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
                And(
                    Identity("A"),
                    Identity("B"));

            var reduced = scope.Reduce(expression);

            Assert.AreEqual(expression, reduced);
        }

        [Test]
        public void ReduceSingleAndEquiv()
        {
            var scope = Scope.Create();

            var expression =
                Equivalence(
                    And(
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
                And(
                    Identity("A"),
                    Identity("A"),
                    Identity("A"));

            var reduced = scope.Reduce(expression);

            Assert.AreEqual(expression, reduced);
        }

        [Test]
        public void ReduceReducibleSingleAndEquiv()
        {
            var scope = Scope.Create();

            var expression =
                Equivalence(
                    And(
                        Identity("A"),
                        Identity("A"),
                        Identity("A")));

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
                Or(
                    Identity("A"),
                    Identity("B"));

            var reduced = scope.Reduce(expression);

            Assert.AreEqual(expression, reduced);
        }

        [Test]
        public void ReduceSingleOrEquiv()
        {
            var scope = Scope.Create();

            var expression =
                Equivalence(
                    Or(
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
                Or(
                    Identity("A"),
                    Identity("A"),
                    Identity("A"));

            var reduced = scope.Reduce(expression);

            Assert.AreEqual(expression, reduced);
        }

        [Test]
        public void ReduceReducibleSingleOrEquiv()
        {
            var scope = Scope.Create();

            var expression =
                Equivalence(
                    Or(
                        Identity("A"),
                        Identity("A"),
                        Identity("A")));

            var reduced = scope.Reduce(expression);

            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
        }
        #endregion

        #region CombinedAndOr
        [Test]
        public void ReduceReducibleCombinedAndOrEquiv()
        {
            var scope = Scope.Create();

            var expression =
                Equivalence(
                    And(
                        Or(
                            Identity("A"),
                            Identity("A")),
                        Or(
                            Identity("A"),
                            Identity("A"))));

            var reduced = scope.Reduce(expression);

            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReduceReducibleCombinedOrAndEquiv()
        {
            var scope = Scope.Create();

            var expression =
                Equivalence(
                    Or(
                        And(
                            Identity("A"),
                            Identity("A")),
                        And(
                            Identity("A"),
                            Identity("A"))));

            var reduced = scope.Reduce(expression);

            var expected =
                Identity("A");

            Assert.AreEqual(expected, reduced);
        }

        [Test]
        public void ReducePartialReducibleCombinedAndOrEquiv()
        {
            var scope = Scope.Create();

            var expression =
                Equivalence(
                    And(
                        Or(
                            Identity("A"),
                            Identity("A")),
                        Or(
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
        public void ReducePartialReducibleCombinedOrAndEquiv()
        {
            var scope = Scope.Create();

            var expression =
                Equivalence(
                    Or(
                        And(
                            Identity("A"),
                            Identity("A")),
                        And(
                            Identity("B"),
                            Identity("B"))));

            var reduced = scope.Reduce(expression);

            var expected =
                Or(
                    Identity("A"),
                    Identity("B"));

            Assert.AreEqual(expected, reduced);
        }
        #endregion
    }
}
