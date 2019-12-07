using NUnit.Framework;

namespace Favalon
{
    [TestFixture]
    class PatternMatchTest
    {
        [TestCase(123, true)]
        [TestCase(456, false)]
        public void MatchBoolean(int result, bool value)
        {
            // match value (true 123) (false 456)
            var term =
                Term.Match(
                    Term.Constant(value),
                    Term.Pair(
                        Term.Constant(true),
                        Term.Constant(123)),
                    Term.Pair(
                        Term.Constant(false),
                        Term.Constant(456)));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((ConstantTerm)actual).Value);
        }

        [TestCase("aaa", 123)]
        [TestCase("bbb", 456)]
        [TestCase("ccc", 0)]
        public void MatchInt32(string result, int value)
        {
            // match value (123 "aaa") (456 "bbb") (0 "ccc")
            var term =
                Term.Match(
                    Term.Constant(value),
                    Term.Pair(
                        Term.Constant(123),
                        Term.Constant("aaa")),
                    Term.Pair(
                        Term.Constant(456),
                        Term.Constant("bbb")),
                    Term.Pair(
                        Term.Constant(0),
                        Term.Constant("ccc")));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((ConstantTerm)actual).Value);
        }

        [TestCase("aaa", 123)]
        [TestCase("bbb", 456)]
        [TestCase("ccc", 1)]
        public void MatchInt32WithUnspecified(string result, int value)
        {
            // match value (123 "aaa") (456 "bbb") (_ "ccc")
            var term =
                Term.Match(
                    Term.Constant(value),
                    Term.Pair(
                        Term.Constant(123),
                        Term.Constant("aaa")),
                    Term.Pair(
                        Term.Constant(456),
                        Term.Constant("bbb")),
                    Term.Pair(
                        Term.Unspecified(),
                        Term.Constant("ccc")));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((ConstantTerm)actual).Value);
        }
    }
}
