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
        public void ReduceSingleAndInData()
        {
            var scope = Scope.Create();

            var expression =
                Equivalence(
                    And(
                        Identity("A"),
                        Identity("B")));

            var reduced = scope.Reduce(expression);

            Assert.AreEqual(expression, reduced);
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
        public void ReduceReducibleSingleAndInData()
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
    }
}
