using Favalon.Terms;
using Favalon.Terms.Algebric;
using Favalon.Terms.Types;
using NUnit.Framework;
using System;
using System.Linq;

namespace Favalon
{
    [TestFixture]
    class TypeTest
    {
        [TestCase(typeof(int), 123)]
        [TestCase(typeof(string), "abc")]
        [TestCase(typeof(bool), false)]
        public void Order1(Type type, object value)
        {
            var term =
                Term.Constant(value);

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual(Term.Type(type), actual.HigherOrder);
        }

        [Test]
        public void ComposeTypeConstructor()
        {
            var term =
                Term.Apply(
                    Term.Type(typeof(Lazy<>)),
                    Term.Type<int>());

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(Term.Type<Lazy<int>>(), actual);
        }

        public sealed class ComposeConstructorTarget
        {
            public readonly int Value;

            public ComposeConstructorTarget(int value) =>
                this.Value = value;
        }

        public void ComposeConstructor()
        {
            var term =
                Term.Apply(
                    Term.Type<ComposeConstructorTarget>(),
                    Term.Constant(123));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(123, ((ComposeConstructorTarget)((ConstantTerm)actual).Value).Value);
        }

        //[Test]
        public void ComposeDiscriminatedUnionType()
        {
            // type ((True 1) (False 0))
            var term =
                Term.DiscriminatedUnionType(
                    Term.Bind(
                        Term.Identity("True"),
                        Term.Identity("True0")),
                    Term.Bind(
                        Term.Identity("False"),
                        Term.Identity("False0")));

            var environment = Environment.Create();
            var inferred = environment.Infer(term);
            var du = (DiscriminatedUnionTypeTerm)environment.Reduce(inferred);

            var True = environment.LookupBoundTerm("True");

            //Assert.AreEqual(, actual.Constructors[0]);
        }

        [Test]
        public void OrTypeTerm()
        {
            // let combined = System.Int32:* | System.String:*
            var term =
                Term.Bind(
                    "combined",
                    Term.Sum(
                        Term.Identity("System.Int32"),
                        Term.Identity("System.String")));

            var environment = Environment.Create();
            environment.SetBoundTerm("System.Int32", Term.Type<int>());
            environment.SetBoundTerm("System.String", Term.Type<string>());

            var actual = environment.Reduce(term);

            Assert.AreEqual(Term.Type<int>(), ((SumTerm)actual).Terms[0]);
            Assert.AreEqual(Term.Type<string>(), ((SumTerm)actual).Terms[1]);
        }

        [Test]
        public void AndTypeTerm()
        {
            // let combined = System.Int32:* & System.String:*
            var term =
                Term.Bind(
                    "combined",
                    Term.Product(
                        Term.Identity("System.Int32"),
                        Term.Identity("System.String")));

            var environment = Environment.Create();
            environment.SetBoundTerm("System.Int32", Term.Type<int>());
            environment.SetBoundTerm("System.String", Term.Type<string>());

            var actual = environment.Reduce(term);

            Assert.AreEqual(Term.Type<int>(), ((ProductTerm)actual).Terms[0]);
            Assert.AreEqual(Term.Type<string>(), ((ProductTerm)actual).Terms[1]);
        }

        private sealed class _1 { }
        private sealed class _2 { }

        // int: int <-- int
        [TestCase(new[] { typeof(int) }, new[] { typeof(int) }, new[] { typeof(int) })]
        // IComparable: IComparable <-- IComparable
        [TestCase(new[] { typeof(IComparable) }, new[] { typeof(IComparable) }, new[] { typeof(IComparable) })]
        // _[1]: _[1] <-- _[1]
        [TestCase(new[] { typeof(_1) }, new[] { typeof(_1) }, new[] { typeof(_1) })]
        // object: object <-- int
        [TestCase(new[] { typeof(object) }, new[] { typeof(object) }, new[] { typeof(int) })]
        // IComparable: IComparable <-- string
        [TestCase(new[] { typeof(IComparable) }, new[] { typeof(IComparable) }, new[] { typeof(string) })]
        // _: _ <-- int
        [TestCase(new[] { typeof(_1) }, new[] { typeof(_1) }, new[] { typeof(int) })]
        // _: _ <-- (int | double)
        [TestCase(new[] { typeof(_1) }, new[] { typeof(_1) }, new[] { typeof(int), typeof(double) })]
        // _[1]: _[1] <-- _[2]
        [TestCase(new[] { typeof(_1) }, new[] { typeof(_1) }, new[] { typeof(_2) })]
        // (int | _): (int | _) <-- string
        [TestCase(new[] { typeof(int), typeof(_1) }, new[] { typeof(int), typeof(_1) }, new[] { typeof(string) })]
        // (int | _): (int | _) <-- (int | string)
        [TestCase(new[] { typeof(int), typeof(_1) }, new[] { typeof(int), typeof(_1) }, new[] { typeof(int), typeof(string) })]
        // (int | _[1]): (int | _[1]) <-- _[2]
        [TestCase(new[] { typeof(int), typeof(_1) }, new[] { typeof(int), typeof(_1) }, new[] { typeof(_2) })]
        // (_[1] | _[2]): (_[1] | _[2]) <-- (_[2] | _[1])
        [TestCase(new[] { typeof(_1), typeof(_2) }, new[] { typeof(_1), typeof(_2) }, new[] { typeof(_2), typeof(_1) })]
        // (int | double): (int | double) <-- (int | double)
        [TestCase(new[] { typeof(int), typeof(double) }, new[] { typeof(int), typeof(double) }, new[] { typeof(int), typeof(double) })]
        // (int | double | string): (int | double | string) <-- (int | double)
        [TestCase(new[] { typeof(int), typeof(double), typeof(string) }, new[] { typeof(int), typeof(double), typeof(string) }, new[] { typeof(int), typeof(double) })]
        // (int | IComparable): (int | IComparable) <-- (int | string)
        [TestCase(new[] { typeof(int), typeof(IComparable) }, new[] { typeof(int), typeof(IComparable) }, new[] { typeof(int), typeof(string) })]
        // null: int <-- (int | double)
        [TestCase(new Type[0], new[] { typeof(int) }, new[] { typeof(int), typeof(double) })]
        // null: (int | double) <-- (int | double | string)
        [TestCase(new Type[0], new[] { typeof(int), typeof(double) }, new[] { typeof(int), typeof(double), typeof(string) })]
        // null: (int | IServiceProvider) <-- (int | double)
        [TestCase(new Type[0], new[] { typeof(int), typeof(IServiceProvider) }, new[] { typeof(int), typeof(double) })]
        // null: int <-- _   [TODO: maybe]
        [TestCase(new Type[0], new[] { typeof(int) }, new[] { typeof(_1) })]
        // (int | double): (int | double) <-- int
        [TestCase(new[] { typeof(int), typeof(double) }, new[] { typeof(int), typeof(double) }, new[] { typeof(int) })]
        // (int | IServiceProvider): (int | IServiceProvider) <-- int
        [TestCase(new[] { typeof(int), typeof(IServiceProvider) }, new[] { typeof(int), typeof(IServiceProvider) }, new[] { typeof(int) })]
        // (int | IComparable): (int | IComparable) <-- string
        [TestCase(new[] { typeof(int), typeof(IComparable) }, new[] { typeof(int), typeof(IComparable) }, new[] { typeof(string) })]
        public void InternalNarrowing(Type[] expectedTypes, Type[] lhsTypes, Type[] rhsTypes)
        {
            Assert.IsTrue(lhsTypes.Length >= 1);
            Assert.IsTrue(rhsTypes.Length >= 1);

            var environment = Environment.Create();
            var p1 = environment.CreatePlaceholder(Term.Unspecified());
            var p2 = environment.CreatePlaceholder(Term.Unspecified());

            Term CreateTermFromType(Type type)
            {
                if (typeof(_1).Equals(type))
                {
                    return p1;
                }
                else if (typeof(_2).Equals(type))
                {
                    return p2;
                }
                else
                {
                    return TypeTerm.From(type);
                }
            }

            var lhs = Term.ComposedSum(lhsTypes.Select(CreateTermFromType))!;
            var rhs = Term.ComposedSum(rhsTypes.Select(CreateTermFromType))!;

            var actual = TypeTerm.Narrow(lhs, rhs);

            var expected = 
                Term.ComposedSum(expectedTypes.Select(CreateTermFromType));

            Assert.AreEqual(expected, actual);
        }
    }
}
