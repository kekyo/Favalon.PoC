using NUnit.Framework;

namespace Favalon.Tokens
{
    [TestFixture]
    public sealed class OperatorTokenTest
    {
        [Test]
        public void Operator()
        {
            var actual = Token.Operator("->");

            Assert.AreEqual("->", actual.ToString());
        }
    }
}
