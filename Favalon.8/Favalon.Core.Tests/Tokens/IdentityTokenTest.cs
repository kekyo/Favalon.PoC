using NUnit.Framework;

namespace Favalon.Tokens
{
    [TestFixture]
    public sealed class IdentityTokenTest
    {
        [Test]
        public void Identity()
        {
            var actual = Token.Identity("abc");

            Assert.AreEqual("abc", actual.ToString());
        }
    }
}
