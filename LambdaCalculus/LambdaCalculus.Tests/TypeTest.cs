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
        // IComparable: IComparable <-- IComparable

        // object: object <-- int
        // IComparable: IComparable <-- string

        // _: _ <-- int
        // _: _ <-- (int | double)
        // _[1]: _[1] <-- _[2]
        // (int | _): (int | _) <-- string
        // (int | _): (int | _) <-- (int | string)
        // (int | _[1]): (int | _[1]) <-- _[2]
        // (_[1] | _[2]): (_[1] | _[2]) <-- (_[2] | _[1])

        // (int | double): (int | double) <-- (int | double)
        // (int | double | string): (int | double | string) <-- (int | double)
        // (int | IComparable): (int | IComparable) <-- (int | string)
        // null: int <-- (int | double)
        // null: (int | double) <-- (int | double | string)
        // null: (int | IServiceProvider) <-- (int | double)

        // null: int <-- _   [TODO: maybe]

        // (int | double): (int | double) <-- int
        // (int | IServiceProvider): (int | IServiceProvider) <-- int
        // (int | IComparable): (int | IComparable) <-- string
        [TestCase(new[] { typeof(int) }, new[] { typeof(int) }, new[] { typeof(int) })]
        [TestCase(new[] { typeof(IComparable) }, new[] { typeof(IComparable) }, new[] { typeof(IComparable) })]
        [TestCase(new[] { typeof(object) }, new[] { typeof(object) }, new[] { typeof(int) })]
        [TestCase(new[] { typeof(IComparable) }, new[] { typeof(IComparable) }, new[] { typeof(string) })]
        [TestCase(new[] { typeof(_1) }, new[] { typeof(_1) }, new[] { typeof(int) })]
        public void InternalNarrowing(Type[] expectedTypes, Type[] lhsTypes, Type[] rhsTypes)
        {
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

            var actual = TypeTerm.Narrow(
                Term.Sum(lhsTypes.Select(CreateTermFromType)),
                Term.Sum(rhsTypes.Select(CreateTermFromType)));

            var expected = 
                Term.Sum(expectedTypes.Select(CreateTermFromType));

            Assert.AreEqual(expected, actual);
        }
    }
}
