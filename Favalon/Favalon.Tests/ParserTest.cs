﻿using NUnit.Framework;
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
        public void ParseInteger()
        {
            var actual = Parser.Parse("123").ToArray();

            var expected = Number("123");
            Assert.AreEqual(expected, actual.Single());
        }

        [Test]
        public void ParseDouble()
        {
            var actual = Parser.Parse("123.456").ToArray();

            var expected = Number("123.456");
            Assert.AreEqual(expected, actual.Single());
        }

        [Test]
        public void ParseString()
        {
            var actual = Parser.Parse("\"abc\"").ToArray();

            var expected = String("abc");
            Assert.AreEqual(expected, actual.Single());
        }

        [Test]
        public void ParseBeforeWhitespace()
        {
            var actual = Parser.Parse(" 123").ToArray();

            var expected = Number("123");
            Assert.AreEqual(expected, actual.Single());
        }

        [Test]
        public void ParseAfterWhitespace()
        {
            var actual = Parser.Parse("123 ").ToArray();

            var expected = Number("123");
            Assert.AreEqual(expected, actual.Single());
        }

        [Test]
        public void ParseMultipleWords()
        {
            var actual = Parser.Parse("abc 123 def").ToArray();

            Assert.AreEqual(new Term[] { Variable("abc"), Number("123"), Variable("def") }, actual);
        }
    }
}
