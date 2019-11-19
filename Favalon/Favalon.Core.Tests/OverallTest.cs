using Favalon.Terms;
using NUnit.Framework;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    public sealed class OverallTest
    {
        //[Test]
        //public void SimpleArrowOperator()
        //{
        //    var text = "a -> b";
        //    var tokens = Lexer.EnumerableTokens(text);

        //    var environment = Environment.Create();
        //    var term = environment.Parse(tokens).
        //        Single();
        //    var transposed = environment.Transpose(term);
        //    var actual = environment.Reduce(transposed);

        //    Assert.AreEqual(
        //        Term.Function(
        //            Term.Identity("a"),
        //            Term.Identity("b")),
        //        actual);
        //}

        //[Test]
        //public void SimpleArrowOperatorWithApplyBody1()
        //{
        //    var text = "a -> b c";
        //    var tokens = Lexer.EnumerableTokens(text);

        //    var environment = Environment.Create();
        //    var term = environment.Parse(tokens).
        //        Single();
        //    var transposed = environment.Transpose(term);
        //    var actual = environment.Reduce(transposed);

        //    Assert.AreEqual(
        //        Term.Function(
        //            Term.Identity("a"),
        //            Term.Apply(
        //                Term.Identity("b"),
        //                Term.Identity("c"))),
        //        actual);
        //}

        //[Test]
        //public void SimpleArrowOperatorWithApplyBody2()
        //{
        //    var text = "a -> (b c)";
        //    var tokens = Lexer.EnumerableTokens(text);

        //    var environment = Environment.Create();
        //    var term = environment.Parse(tokens).
        //        Single();
        //    var transposed = environment.Transpose(term);
        //    var actual = environment.Reduce(transposed);

        //    Assert.AreEqual(
        //        Term.Function(
        //            Term.Identity("a"),
        //            Term.Apply(
        //                Term.Identity("b"),
        //                Term.Identity("c"))),
        //        actual);
        //}

        //[Test]
        //public void SimpleArrowOperatorDouble()
        //{
        //    var text = "a -> b -> c";
        //    var tokens = Lexer.EnumerableTokens(text);

        //    var environment = Environment.Create();
        //    var term = environment.Parse(tokens).
        //        Single();
        //    var transposed = environment.Transpose(term);
        //    var actual = environment.Reduce(transposed);

        //    Assert.AreEqual(
        //        Term.Function(
        //            Term.Identity("a"),
        //            Term.Function(
        //                Term.Identity("b"),
        //                Term.Identity("c"))),
        //        actual);
        //}

        //[Test]
        //public void SimpleArrowOperatorDoubleWithApply()
        //{
        //    var text = "a -> b -> c d";
        //    var tokens = Lexer.EnumerableTokens(text);

        //    var environment = Environment.Create();
        //    var term = environment.Parse(tokens).
        //        Single();
        //    var transposed = environment.Transpose(term);
        //    var actual = environment.Reduce(transposed);

        //    Assert.AreEqual(
        //        Term.Function(
        //            Term.Identity("a"),
        //            Term.Function(
        //                Term.Identity("b"),
        //                Term.Apply(
        //                    Term.Identity("c"),
        //                    Term.Identity("d")))),
        //        actual);
        //}

        ////////////////////////////////////////////////////

        //[Test]
        //public void EnumerableIdentityToken11()
        //{
        //    var text = "(x -> x) y -> y";
        //    var tokens = Lexer.EnumerableTokens(text);

        //    var environment = Environment.Create();
        //    var term = environment.Parse(tokens).
        //        Single();
        //    var transposed = environment.Transpose(term);
        //    var actual = environment.Reduce(transposed);

        //    Assert.AreEqual(
        //        Term.Function(
        //            Term.Identity("y"),
        //            Term.Identity("y")),
        //        actual);
        //}

        //[Test]
        //public void EnumerableIdentityToken12()
        //{
        //    var text = "(x -> x x) (y -> y)";
        //    var tokens = Lexer.EnumerableTokens(text);

        //    var environment = Environment.Create();
        //    var term = environment.Parse(tokens).
        //        Single();
        //    var transposed = environment.Transpose(term);
        //    var actual1 = environment.Reduce(transposed, false);
        //    var actual2 = environment.Reduce(actual1, false);

        //    Assert.AreEqual(
        //        Term.Apply(
        //            Term.Function(
        //                Term.Identity("y"),
        //                Term.Identity("y")),
        //            Term.Function(
        //                Term.Identity("y"),
        //                Term.Identity("y"))),
        //        actual1);

        //    Assert.AreEqual(
        //        Term.Function(
        //            Term.Identity("y"),
        //            Term.Identity("y")),
        //        actual2);
        //}
    }
}
