using LambdaCalculus.Operators;
using NUnit.Framework;
using System;

namespace LambdaCalculus
{
    [TestFixture]
    class TypeTest
    {
        [TestCase(typeof(int), 123)]
        [TestCase(typeof(string), "abc")]
        [TestCase(typeof(bool), false)]
        public void Order1Test(Type type, object value)
        {
            var term =
                Term.Constant(value);

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual(Term.Type(type), actual.HigherOrder);
        }
    }
}
