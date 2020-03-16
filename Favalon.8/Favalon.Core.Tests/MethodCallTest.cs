using Favalon.Terms;
using NUnit.Framework;

namespace Favalon
{
    [TestFixture]
    public sealed class MethodCallTest
    {
        [Test]
        public void SystemInt32Parse()
        {
            var term = Term.Apply(Term.Identity("System.Int32.Parse"), Term.Constant("123"));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(
                Term.Constant(123),
                actual);
        }
    }
}
