using Favalon.Terms;
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
            // match ((true 123) (false 456)) value
            var term =
                Term.Apply(
                    Term.Match(
                        Term.Pair(
                            Term.Constant(true),
                            Term.Constant(123)),
                        Term.Pair(
                            Term.Constant(false),
                            Term.Constant(456))),
                    Term.Constant(value));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((ConstantTerm)actual).Value);
            Assert.AreEqual(Term.Type<int>(), actual.HigherOrder);
        }

        [TestCase("aaa", 123)]
        [TestCase("bbb", 456)]
        [TestCase("ccc", 0)]
        public void MatchInt32(string result, int value)
        {
            // match ((123 "aaa") (456 "bbb") (0 "ccc")) value
            var term =
                Term.Apply(
                    Term.Match(
                        Term.Pair(
                            Term.Constant(123),
                            Term.Constant("aaa")),
                        Term.Pair(
                            Term.Constant(456),
                            Term.Constant("bbb")),
                        Term.Pair(
                            Term.Constant(0),
                            Term.Constant("ccc"))),
                    Term.Constant(value));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((ConstantTerm)actual).Value);
            Assert.AreEqual(Term.Type<string>(), actual.HigherOrder);
        }

        [TestCase("aaa", 123)]
        [TestCase("bbb", 456)]
        [TestCase("ccc", 1)]
        public void MatchInt32WithUnspecified(string result, int value)
        {
            // match ((123 "aaa") (456 "bbb") (_ "ccc")) value
            var term =
                Term.Apply(
                    Term.Match(
                        Term.Pair(
                            Term.Constant(123),
                            Term.Constant("aaa")),
                        Term.Pair(
                            Term.Constant(456),
                            Term.Constant("bbb")),
                        Term.Pair(
                            Term.Unspecified(),
                            Term.Constant("ccc"))),
                    Term.Constant(value));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(result, ((ConstantTerm)actual).Value);
            Assert.AreEqual(Term.Type<string>(), actual.HigherOrder);
        }
    }
}
