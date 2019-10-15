using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Favalon
{
    using static Favalon.Terms.Factories;

    [TestFixture]
    public sealed class InferrerTest
    {
        [Test]
        public void InferNotBoundVariable()
        {
            var environment = Environment.Create();

            var actual = environment.Infer(Variable("abc"));
            
            Assert.AreEqual(Variable("abc"), actual);
        }

        [Test]
        public void InferBoundVariable()
        {
            var environment = Environment.Create();
            var environment2 = environment.Bind("abc", Number(123));

            var actual = environment2.Infer(Variable("abc"));

            Assert.AreEqual(Number(123), actual);
        }

        [Test]
        public void InferIntegerNumber()
        {
            var environment = Environment.Create();

            var actual = environment.Infer(Number("123"));

            var expected = Number(123);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void InferDoubleNumber()
        {
            var environment = Environment.Create();

            var actual = environment.Infer(Number("123.456"));

            var expected = Number(123.456);
            Assert.AreEqual(expected, actual);
        }

        //[Test]
        //public void InferDefaultBoundArrow()
        //{
        //    var environment = Environment.Create();

        //    var actual = environment.Infer(Variable("->"));

        //    var expected = environment.BoundTerms["->"];
        //    Assert.AreEqual(expected, actual);
        //}

        [Test]
        public void InferDotNetFunction()
        {
            var environment = Environment.Create();

            var actual = environment.Infer(
                Apply(Variable("System.Int32.Parse"), String("123")));

            var parse = typeof(int).GetMethod("Parse", new[] { typeof(string) })!;

            var expected = Apply(MethodSymbol(parse), String("123"));
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void InferExecutableFunction()
        {
            var environment = Environment.Create();

            var actual = environment.Infer(
                Apply(Variable("wc"), String("abcde fghij")));

            var expected = Apply(ExecutableSymbol(@"C:\Program Files\Git\usr\bin\wc.exe"), String("abcde fghij"));
            Assert.AreEqual(expected, actual);
        }
    }
}
