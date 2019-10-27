using NUnit.Framework;

namespace Favalon.Tokens
{
    [TestFixture]
    public sealed class BeginBracketTokenTest
    {
        [Test]
        public void Begin()
        {
            var actual = Token.Begin();

            Assert.AreEqual("(", actual.ToString());
        }
    }
}
