using NUnit.Framework;

namespace Favalon.Tokens
{
    [TestFixture]
    public sealed class NumericalSignTokenTest
    {
        [Test]
        public void NumericalSign()
        {
            var actual = Token.NumericalSign('+');

            Assert.AreEqual("+", actual.ToString());
        }
    }
}
