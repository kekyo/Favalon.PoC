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
        public void Single()
        {
            var actual = Parser.Parse(new Term[] { Variable("abc") });

            Assert.AreEqual(Variable("abc"), actual);
        }

        [Test]
        public void Apply1()
        {
            var actual = Parser.Parse(new Term[] { Variable("abc"), Number(123) });

            Assert.AreEqual(Apply(Variable("abc"), Number(123)), actual);
        }

        [Test]
        public void Apply2()
        {
            var actual = Parser.Parse(new Term[] { Variable("abc"), Number(123), String("aaa") });

            Assert.AreEqual(Apply(Apply(Variable("abc"), Number(123)), String("aaa")), actual);
        }
    }
}
