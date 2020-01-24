using Favalon.Terms;
using NUnit.Framework;

using static Favalon.TermFactory;
using static Favalon.ClrTermFactory;

namespace Favalon
{
    //[TestFixture]
    //class PatternMatchTest
    //{
    //    [TestCase(123, true)]
    //    [TestCase(456, false)]
    //    public void MatchBoolean(int result, bool value)
    //    {
    //        // match ((true 123) (false 456)) value
    //        var term =
    //            Apply(
    //                Match(
    //                    Pair(
    //                        Constant(true),
    //                        Constant(123)),
    //                    Pair(
    //                        Constant(false),
    //                        Constant(456))),
    //                Constant(value));

    //        var environment = Environment.Create();
    //        var actual = environment.Reduce(term);

    //        Assert.AreEqual(result, ((ConstantTerm)actual).Value);
    //        Assert.AreEqual(Type<int>(), actual.HigherOrder);
    //    }

    //    [TestCase("aaa", 123)]
    //    [TestCase("bbb", 456)]
    //    [TestCase("ccc", 0)]
    //    public void MatchInt32(string result, int value)
    //    {
    //        // match ((123 "aaa") (456 "bbb") (0 "ccc")) value
    //        var term =
    //            Apply(
    //                Match(
    //                    Pair(
    //                        Constant(123),
    //                        Constant("aaa")),
    //                    Pair(
    //                        Constant(456),
    //                        Constant("bbb")),
    //                    Pair(
    //                        Constant(0),
    //                        Constant("ccc"))),
    //                Constant(value));

    //        var environment = Environment.Create();
    //        var actual = environment.Reduce(term);

    //        Assert.AreEqual(result, ((ConstantTerm)actual).Value);
    //        Assert.AreEqual(Type<string>(), actual.HigherOrder);
    //    }

    //    [TestCase("aaa", 123)]
    //    [TestCase("bbb", 456)]
    //    [TestCase("ccc", 1)]
    //    public void MatchInt32WithUnspecified(string result, int value)
    //    {
    //        // match ((123 "aaa") (456 "bbb") (_ "ccc")) value
    //        var term =
    //            Apply(
    //                Match(
    //                    Pair(
    //                        Constant(123),
    //                        Constant("aaa")),
    //                    Pair(
    //                        Constant(456),
    //                        Constant("bbb")),
    //                    Pair(
    //                        Unspecified(),
    //                        Constant("ccc"))),
    //                Constant(value));

    //        var environment = Environment.Create();
    //        var actual = environment.Reduce(term);

    //        Assert.AreEqual(result, ((ConstantTerm)actual).Value);
    //        Assert.AreEqual(Type<string>(), actual.HigherOrder);
    //    }
    //}
}
