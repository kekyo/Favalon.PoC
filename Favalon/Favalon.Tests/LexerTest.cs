using Favalon.Expression;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Favalon
{
    using static Favalon.Expression.Factories;

    [TestFixture]
    public sealed class LexerTest
    {
        [Test]
        public void BufferingBoundary()
        {
            var line = "abc 123 \"aaa\" 456.789 def";
            var expected = new Term[] { Variable("abc"), Number("123"), String("aaa"), Number("456.789"), Variable("def") };

            for (var bufferSize = 1; bufferSize <= (line.Length + 1); bufferSize++)
            {
                var actual = Lexer.Lex(new StringReader(line), bufferSize).ToArray();

                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void LexVariable()
        {
            var actual = Lexer.Lex("abc").ToArray();

            Assert.AreEqual(Variable("abc"), actual.Single());
        }

        [Test]
        public void LexInteger()
        {
            var actual = Lexer.Lex("123").ToArray();

            var expected = Number("123");
            Assert.AreEqual(expected, actual.Single());
        }

        [Test]
        public void LexDouble()
        {
            var actual = Lexer.Lex("123.456").ToArray();

            var expected = Number("123.456");
            Assert.AreEqual(expected, actual.Single());
        }

        [Test]
        public void LexString()
        {
            var actual = Lexer.Lex("\"abc\"").ToArray();

            var expected = String("abc");
            Assert.AreEqual(expected, actual.Single());
        }

        [Test]
        public void LexBeforeWhitespace()
        {
            var actual = Lexer.Lex(" 123").ToArray();

            var expected = Number("123");
            Assert.AreEqual(expected, actual.Single());
        }

        [Test]
        public void LexAfterWhitespace()
        {
            var actual = Lexer.Lex("123 ").ToArray();

            var expected = Number("123");
            Assert.AreEqual(expected, actual.Single());
        }

        [Test]
        public void LexMultipleWords()
        {
            var actual = Lexer.Lex("abc 123 def").ToArray();

            Assert.AreEqual(new Term[] { Variable("abc"), Number("123"), Variable("def") }, actual);
        }
    }
}
