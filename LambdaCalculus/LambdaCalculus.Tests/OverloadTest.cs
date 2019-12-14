using Favalon.Terms;
using Favalon.Terms.Operators;
using Favalon.Terms.Types;
using NUnit.Framework;
using System;

namespace Favalon
{
    [TestFixture]
    class OverloadTest
    {
        [Test]
        public void NonOverloadedMethod()
        {
            var term =
                Term.Apply(
                    Term.Method(typeof(int).GetMethod("Parse", new[] { typeof(string) })),
                    Term.Constant("123"));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(Term.Constant(123), actual);
        }
    }
}
