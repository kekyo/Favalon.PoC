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
    }
}
