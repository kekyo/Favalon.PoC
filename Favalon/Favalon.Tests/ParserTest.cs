using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using static Favalon.Factories;

namespace Favalon
{
    [TestFixture]
    public sealed class ParserTest
    {
        [Test]
        public void BufferingIterator()
        {
            var line = "abc 123 \"aaa\" 456.789 def";
            var expected = new Term[] { Variable("abc"), Number("123"), String("aaa"), Number("456.789"), Variable("def") };

            for (var bufferSize = 1; bufferSize <= (line.Length + 1); bufferSize++)
            {
                var actual = Parser.Parse(new StringReader(line), bufferSize).ToArray();

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void ParseVariable()
        {
            var actual = Parser.Parse("abc").ToArray();

            Assert.AreEqual(Variable("abc"), actual.Single());
        }

        [Test]
        public void ParseNumber()
        {
            var actual = Parser.Parse("123").ToArray();

            var expected = Number("123");
            Assert.AreEqual(expected, actual.Single());
        }

        [Test]
        public void ParseArrow()
        {
            var actual = Parser.Parse("->").ToArray();

            var expected = Variable("->");
            Assert.AreEqual(expected, actual.Single());
        }

        [Test]
        public void ParseApplyVariableAndNumber()
        {
            var actual = Parser.Parse(new StringReader("abc 123"), 5).ToArray();

            Assert.AreEqual(new Term[] { Variable("abc"), Number("123") }, actual);
        }
    }
}
