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
    public sealed class ReduceTest
    {
        [Test]
        public void DotNetMethod()
        {
            var parse = typeof(int).GetMethod("Parse", new[] { typeof(string) })!;
            var term = Apply(MethodSymbol(parse), String("123"));

            var actual = term.Reduce();

            Assert.AreEqual(123, actual);
        }

        [Test]
        public void ExecutableMethod()
        {
            var term = Apply(ExecutableSymbol(@"C:\Program Files\Git\usr\bin\wc.exe"), String("abcde fghij"));

            object actual = term.Reduce();

            var splitted = ((string)actual).Split(
                new[] { ' ', '\r', '\n' },
                StringSplitOptions.RemoveEmptyEntries);

            // Print newline, word, and byte counts 
            Assert.AreEqual(new[] { "0", "2", "11" }, splitted);
        }
    }
}
