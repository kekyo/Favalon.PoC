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

            var expected = Number("123");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseArrow()
        {
            var actual = Parser.Parse("->");

            var expected = Variable("->");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ParseApplyVariableAndNumber()
        {
            // abc: int -> string
            // abc 123: string

            var actual = Parser.Parse("abc 123");

            var actualForApply = (Apply)actual;
            Assert.AreEqual(Variable("abc"), actualForApply.Function);
            Assert.AreEqual(Number("123"), actualForApply.Argument);
        }
    }
}
