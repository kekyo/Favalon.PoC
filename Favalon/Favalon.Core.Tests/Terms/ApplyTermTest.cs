using NUnit.Framework;

namespace Favalon.Terms
{
    [TestFixture]
    public sealed class ApplyTermTest
    {
        [Test]
        public void Apply()
        {
            var actual = Term.Apply(
                Term.Identity("x"),
                Term.Identity("y"));

            Assert.AreEqual("x y", actual.Readable);
        }

        [Test]
        public void ReplaceAtFunction()
        {
            var a = Term.Apply(
                Term.Identity("x"),
                Term.Identity("y"));

            var environment = Environment.Create();
            var actual = environment.Replace(a, "x", Term.Identity("z"));

            Assert.AreEqual("z y", actual.Readable);
        }

        [Test]
        public void ReplaceAtArgument()
        {
            var a = Term.Apply(
                Term.Identity("x"),
                Term.Identity("y"));

            var environment = Environment.Create();
            var actual = environment.Replace(a, "y", Term.Identity("z"));

            Assert.AreEqual("x z", actual.Readable);
        }

        [Test]
        public void ReplaceNotApplicable()
        {
            var a = Term.Apply(
                Term.Identity("x"),
                Term.Identity("y"));

            var environment = Environment.Create();
            var actual = environment.Replace(a, "z", Term.Identity("q"));

            Assert.AreEqual("x y", actual.Readable);
        }

        [Test]
        public void ReduceNotReduceable()
        {
            var a = Term.Apply(
                Term.Identity("x"),
                Term.Identity("y"));

            var environment = Environment.Create();
            var actual = environment.Reduce(a);

            Assert.AreEqual("x y", actual.Readable);
        }

        [Test]
        public void ReduceFunctionByApplied()
        {
            // (-> x x) y
            var a = Term.Apply(
                Term.Function(
                    Term.Identity("x"),
                    Term.Identity("x")),
                Term.Identity("y"));

            var environment = Environment.Create();
            var actual = environment.Reduce(a);

            Assert.AreEqual("y", actual.Readable);
        }

        [Test]
        public void ReduceArrow()
        {
            // -> a b
            var a = Term.Apply(
                Term.Apply(
                    Term.Identity("->"),
                    Term.Identity("a")),
                Term.Identity("b"));

            var environment = Environment.Create();
            var actual = environment.Reduce(a);

            Assert.AreEqual("a -> b", actual.Readable);
        }

        [Test]
        public void ReduceArrowAndApply()
        {
            // -> a (b c)
            var a = Term.Apply(
                Term.Apply(
                    Term.Identity("->"),
                    Term.Identity("a")),
                Term.Apply(
                    Term.Identity("b"),
                    Term.Identity("c")));

            var environment = Environment.Create();
            var actual = environment.Reduce(a);

            Assert.AreEqual("a -> b c", actual.Readable);
        }

        [Test]
        public void ReduceArrowAndApply2()
        {
            // -> a (b c d)
            var a = Term.Apply(
                Term.Apply(
                    Term.Identity("->"),
                    Term.Identity("a")),
                Term.Apply(
                    Term.Apply(
                        Term.Identity("b"),
                        Term.Identity("c")),
                    Term.Identity("d")));

            var environment = Environment.Create();
            var actual = environment.Reduce(a);

            Assert.AreEqual("a -> b c d", actual.Readable);
        }

        [Test]
        public void ReduceApplyAndArrow()
        {
            // a (-> b c)
            var a = Term.Apply(
                Term.Identity("a"),
                Term.Apply(
                    Term.Apply(
                        Term.Identity("->"),
                        Term.Identity("b")),
                    Term.Identity("c")));

            var environment = Environment.Create();
            var actual = environment.Reduce(a);

            Assert.AreEqual("a (b -> c)", actual.Readable);
        }

        [Test]
        public void ReduceApplyAndArrowAndApply()
        {
            // a (-> b (c d))
            var a = Term.Apply(
                Term.Identity("a"),
                Term.Apply(
                    Term.Apply(
                        Term.Identity("->"),
                        Term.Identity("b")),
                    Term.Apply(
                        Term.Identity("c"),
                        Term.Identity("d"))));

            var environment = Environment.Create();
            var actual = environment.Reduce(a);

            Assert.AreEqual("a (b -> c d)", actual.Readable);
        }
    }
}
