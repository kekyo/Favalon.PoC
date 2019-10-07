using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

using static Favalon.Factories;

namespace Favalon
{
    [TestFixture]
    public sealed class ParserTest
    {
        [Test]
        public void ParseVariable()
        {
            var actual = Parser.Parse("abc");

            Assert.AreEqual(Variable("abc"), actual);
        }

        [Test]
        public void ParseNumber()
        {
            var actual = Parser.Parse("123");

            var expected = Number(123);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(ClrType<int>(), actual.HigherOrder);
        }

        [Test]
        public void ParseApplyVariableAndNumber()
        {
            var actual = Parser.Parse("abc 123");

            Assert.AreEqual(Apply(Variable("abc"), Number(123)), actual);
        }
    }
}
