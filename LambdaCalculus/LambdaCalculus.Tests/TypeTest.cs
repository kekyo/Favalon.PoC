using Favalon.Terms;
using Favalon.Terms.Algebraic;
using Favalon.Terms.Types;
using NUnit.Framework;
using System;
using System.Collections;
using System.Linq;

using static Favalon.TermFactory;
using static Favalon.ClrTermFactory;

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
                Constant(value);

            var environment = Environment.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual(Constant(type), actual.HigherOrder);
        }

        [Test]
        public void ComposeTypeConstructor()
        {
            var term =
                Apply(
                    Constant(typeof(Lazy<>)),
                    Type<int>());

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(Type<Lazy<int>>(), actual);
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
                Apply(
                    Type<ComposeConstructorTarget>(),
                    Constant(123));

            var environment = Environment.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(123, ((ComposeConstructorTarget)((ConstantTerm)actual).Value).Value);
        }

        [Test]
        public void SumTypeTerm()
        {
            // let combined = System.Int32:* + System.String:*
            var term =
                Bind(
                    "combined",
                    Sum(
                        Identity("System.Int32"),
                        Identity("System.String")));

            var environment = ClrEnvironment.Create();

            var actual = environment.Reduce(term);

            Assert.AreEqual(Type<int>(), ((SumTerm)actual).Flatten().ElementAt(0));
            Assert.AreEqual(Type<string>(), ((SumTerm)actual).Flatten().ElementAt(1));
        }

        [Test]
        public void SumToCombinedTypeTerm()
        {
            // let combined = System.Int32:* + System.IFormattable:*
            // --> System.IFormattable:*
            var term =
                Bind(
                    "combined",
                    Sum(
                        Identity("System.Int32"),
                        Identity("System.IFormattable")));

            var environment = ClrEnvironment.Create();

            var actual = environment.Reduce(term);

            Assert.AreEqual(Type<IFormattable>(), actual);
        }

        [Test]
        public void ProductTypeTerm()
        {
            // let combined = System.Int32:* * System.String:*
            var term =
                Bind(
                    "combined",
                    Product(
                        Identity("System.Int32"),
                        Identity("System.String")));

            var environment = ClrEnvironment.Create();

            var actual = environment.Reduce(term);

            Assert.AreEqual(Type<int>(), ((ProductTerm)actual).Flatten().ElementAt(0));
            Assert.AreEqual(Type<string>(), ((ProductTerm)actual).Flatten().ElementAt(1));
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
        // _: _ <-- (int + double)
        [TestCase(new[] { typeof(_1) }, new[] { typeof(_1) }, new[] { typeof(int), typeof(double) })]
        // _[1]: _[1] <-- _[2]
        [TestCase(new[] { typeof(_1) }, new[] { typeof(_1) }, new[] { typeof(_2) })]
        // (int + _): (int + _) <-- string
        [TestCase(new[] { typeof(int), typeof(_1) }, new[] { typeof(int), typeof(_1) }, new[] { typeof(string) })]
        // (int + _): (int + _) <-- (int + string)
        [TestCase(new[] { typeof(int), typeof(_1) }, new[] { typeof(int), typeof(_1) }, new[] { typeof(int), typeof(string) })]
        // (int + _[1]): (int + _[1]) <-- _[2]
        [TestCase(new[] { typeof(int), typeof(_1) }, new[] { typeof(int), typeof(_1) }, new[] { typeof(_2) })]
        // (_[1] + _[2]): (_[1] + _[2]) <-- (_[2] + _[1])
        [TestCase(new[] { typeof(_1), typeof(_2) }, new[] { typeof(_1), typeof(_2) }, new[] { typeof(_2), typeof(_1) })]
        // (int + double): (int + double) <-- (int + double)
        [TestCase(new[] { typeof(int), typeof(double) }, new[] { typeof(int), typeof(double) }, new[] { typeof(int), typeof(double) })]
        // (int + double + string): (int + double + string) <-- (int + double)
        [TestCase(new[] { typeof(int), typeof(double), typeof(string) }, new[] { typeof(int), typeof(double), typeof(string) }, new[] { typeof(int), typeof(double) })]
        // (int + IComparable): (int + IComparable) <-- (int + string)
        [TestCase(new[] { typeof(int), typeof(IComparable) }, new[] { typeof(int), typeof(IComparable) }, new[] { typeof(int), typeof(string) })]
        // null: int <-- (int + double)
        [TestCase(new Type[0], new[] { typeof(int) }, new[] { typeof(int), typeof(double) })]
        // null: (int + double) <-- (int + double + string)
        [TestCase(new Type[0], new[] { typeof(int), typeof(double) }, new[] { typeof(int), typeof(double), typeof(string) })]
        // null: (int + IServiceProvider) <-- (int + double)
        [TestCase(new Type[0], new[] { typeof(int), typeof(IServiceProvider) }, new[] { typeof(int), typeof(double) })]
        // null: int <-- _   [TODO: maybe]
        [TestCase(new Type[0], new[] { typeof(int) }, new[] { typeof(_1) })]
        // (int + double): (int + double) <-- int
        [TestCase(new[] { typeof(int), typeof(double) }, new[] { typeof(int), typeof(double) }, new[] { typeof(int) })]
        // (int + IServiceProvider): (int + IServiceProvider) <-- int
        [TestCase(new[] { typeof(int), typeof(IServiceProvider) }, new[] { typeof(int), typeof(IServiceProvider) }, new[] { typeof(int) })]
        // (int + IComparable): (int + IComparable) <-- string
        [TestCase(new[] { typeof(int), typeof(IComparable) }, new[] { typeof(int), typeof(IComparable) }, new[] { typeof(string) })]
        public void InternalWidening(Type[] expectedTypes, Type[] lhsTypes, Type[] rhsTypes)
        {
            Assert.IsTrue(lhsTypes.Length >= 1);
            Assert.IsTrue(rhsTypes.Length >= 1);

            var environment = ClrEnvironment.Create();
            var p1 = environment.CreatePlaceholder(Unspecified());
            var p2 = environment.CreatePlaceholder(Unspecified());

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
                    return Constant(type);
                }
            }

            var lhs = SumType(lhsTypes.Select(CreateTermFromType))!;
            var rhs = SumType(rhsTypes.Select(CreateTermFromType))!;

            var actual = ClrTypeCalculator.Instance.Widening(lhs, rhs);

            var expected = SumType(expectedTypes.Select(CreateTermFromType));

            Assert.AreEqual(expected, actual);
        }

        [TestCase(
            new[] { typeof(byte), typeof(short), typeof(int), typeof(long) },
            new[] { typeof(short), typeof(long), typeof(byte), typeof(int) })]
        [TestCase(
            new[] { typeof(int), typeof(long), typeof(sbyte), typeof(ushort) },
            new[] { typeof(ushort), typeof(long), typeof(sbyte), typeof(int) })]
        [TestCase(
            new[] { typeof(byte), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double) },
            new[] { typeof(short), typeof(double), typeof(long), typeof(float), typeof(byte), typeof(int) })]
        [TestCase(
            new[] { typeof(byte), typeof(short), typeof(int), typeof(long), typeof(DateTime), typeof(Guid) },
            new[] { typeof(short), typeof(Guid), typeof(long), typeof(DateTime), typeof(byte), typeof(int) })]
        [TestCase(
            new[] { typeof(string), typeof(object) },
            new[] { typeof(object), typeof(string) })]
        [TestCase(
            new[] { typeof(string), typeof(IComparable) },
            new[] { typeof(IComparable), typeof(string) })]
        [TestCase(
            new[] { typeof(string), typeof(IComparable), typeof(object) },
            new[] { typeof(IComparable), typeof(object), typeof(string) })]
        [TestCase(
            new[] { typeof(int), typeof(string), typeof(object) },
            new[] { typeof(object), typeof(string), typeof(int) })]
        [TestCase(
            new[] { typeof(int[]), typeof(Array), typeof(ICollection) },
            new[] { typeof(ICollection), typeof(int[]), typeof(Array) })]
        [TestCase(
            new[] { typeof(IList), typeof(ICollection), typeof(IEnumerable) },
            new[] { typeof(ICollection), typeof(IList), typeof(IEnumerable) })]
        [TestCase(
            new[] { typeof(DateTime), typeof(Uri) },
            new[] { typeof(Uri), typeof(DateTime) })]
        [TestCase(
            new[] { typeof(DateTime), typeof(Guid), typeof(Uri) },
            new[] { typeof(Uri), typeof(DateTime), typeof(Guid) })]
        [TestCase(
            new[] { typeof(Guid), typeof(DateTime), typeof(Uri) },
            new[] { typeof(Uri), typeof(Guid), typeof(DateTime) })]
        public void ConcreterComparer(Type[] expectedTypes, Type[] targetTypes)
        {
            var actual = targetTypes.Select(
                targetType => Constant(targetType)).
                OrderBy(term => term, ClrTypeCalculator.Instance).
                ToArray();

            var expected = expectedTypes.Select(
                targetType => Constant(targetType)).
                ToArray();

            Assert.AreEqual(expected, actual);
        }
    }
}
