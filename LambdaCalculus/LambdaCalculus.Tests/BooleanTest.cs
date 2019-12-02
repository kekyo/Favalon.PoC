using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LambdaCalculus
{
    [TestFixture]
    class BooleanTest
    {
        [Test]
        public void BooleanTrue()
        {
            var term = new BooleanTerm(true);

            var actual = term.Reduce();

            Assert.AreEqual(true, ((BooleanTerm)actual).Value);
        }

        [Test]
        public void BooleanFalse()
        {
            var term = new BooleanTerm(false);

            var actual = term.Reduce();

            Assert.AreEqual(false, ((BooleanTerm)actual).Value);
        }

        [TestCase(true, true, true)]
        [TestCase(false, false, true)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public void BooleanAnd(bool result, bool lhs, bool rhs)
        {
            var term =
                new ApplyTerm(
                    new ApplyTerm(
                        new AndTerm(),
                        new BooleanTerm(lhs)),
                    new BooleanTerm(rhs));

            var actual = term.Reduce();

            Assert.AreEqual(result, ((BooleanTerm)actual).Value);
        }
    }
}
