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

            Assert.AreEqual("x y", actual.ToString());
        }

        [Test]
        public void ReplaceAtFunction()
        {
            var a = Term.Apply(
                Term.Identity("x"),
                Term.Identity("y"));
            var actual = a.VisitReplace("x", Term.Identity("z"));

            Assert.AreEqual("z y", actual.ToString());
        }

        [Test]
        public void ReplaceAtArgument()
        {
            var a = Term.Apply(
                Term.Identity("x"),
                Term.Identity("y"));
            var actual = a.VisitReplace("y", Term.Identity("z"));

            Assert.AreEqual("x z", actual.ToString());
        }

        [Test]
        public void ReplaceNotApplicable()
        {
            var a = Term.Apply(
                Term.Identity("x"),
                Term.Identity("y"));
            var actual = a.VisitReplace("z", Term.Identity("q"));

            Assert.AreEqual("x y", actual.ToString());
        }

        [Test]
        public void ReduceNotReduceable()
        {
            var a = Term.Apply(
                Term.Identity("x"),
                Term.Identity("y"));

            var environment = Environment.Create();
            var actual = environment.Reduce(a);

            Assert.AreEqual("x y", actual.ToString());
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

            Assert.AreEqual("y", actual.ToString());
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

            Assert.AreEqual("a -> b", actual.ToString());
        }

        [Test]
        public void ReduceArrowAndApply()
        {
            // -> a b c
            var a = Term.Apply(
                Term.Apply(
                    Term.Apply(
                        Term.Identity("->"),
                        Term.Identity("a")),
                    Term.Identity("b")),
                Term.Identity("c"));

            var environment = Environment.Create();
            var actual = environment.Reduce(a);

            Assert.AreEqual("a -> b c", actual.ToString());
        }

        [Test]
        public void ReduceArrowAndApply2()
        {
            // -> a b c d
            var a = Term.Apply(
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("->"),
                            Term.Identity("a")),
                        Term.Identity("b")),
                    Term.Identity("c")),
                Term.Identity("d"));

            var environment = Environment.Create();
            var actual = environment.Reduce(a);

            Assert.AreEqual("a -> b c d", actual.ToString());
        }

        [Test]
        public void ReduceApplyAndArrow()
        {
            // a -> b c
            var a = Term.Apply(
                Term.Apply(
                    Term.Apply(
                        Term.Identity("a"),
                        Term.Identity("->")),
                    Term.Identity("b")),
                Term.Identity("c"));

            var environment = Environment.Create();
            var actual = environment.Reduce(a);

            Assert.AreEqual("a (b -> c)", actual.ToString());
        }

        [Test]
        public void ReduceApplyAndArrowAndApply()
        {
            // a -> b c d
            var a = Term.Apply(
                Term.Apply(
                    Term.Apply(
                        Term.Apply(
                            Term.Identity("a"),
                            Term.Identity("->")),
                        Term.Identity("b")),
                    Term.Identity("c")),
                Term.Identity("d"));

            var environment = Environment.Create();
            var actual = environment.Reduce(a);

            Assert.AreEqual("a (b -> c d)", actual.ToString());
        }
    }
}
