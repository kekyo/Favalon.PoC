using NUnit.Framework;

namespace Favalon.Tokens
{
    [TestFixture]
    public sealed class EndBracketTokenTest
    {
        [Test]
        public void End()
        {
            var actual = Token.End();

            Assert.AreEqual(")", actual.ToString());
        }
    }
}
