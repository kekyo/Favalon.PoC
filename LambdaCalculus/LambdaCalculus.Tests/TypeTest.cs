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

            var environment = EnvironmentFactory.Create();
            var actual = environment.Infer(term);

            Assert.AreEqual(Constant(type), actual.HigherOrder);
        }

        [Test]
        public void ComposeTypeConstructor()
        {
            var term =
                Apply(
                    Constant(typeof(Lazy<>)),
                    ClrType<int>());

            var environment = EnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(ClrType<Lazy<int>>(), actual);
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
                    ClrType<ComposeConstructorTarget>(),
                    Constant(123));

            var environment = EnvironmentFactory.Create();
            var actual = environment.Reduce(term);

            Assert.AreEqual(123, ((ComposeConstructorTarget)((ClrConstantTerm)actual).Value).Value);
        }

        [Test]
        public void SumTypeTerm()
        {
            // let combined = System.Int32:* + System.String:*
            var term =
                BindMutable(
                    "combined",
                    Sum(
                        Identity("System.Int32"),
                        Identity("System.String")));

            var environment = ClrEnvironmentFactory.Create();

            var actual = environment.Reduce(term);

            Assert.AreEqual(ClrType<int>(), ((SumTerm)actual).Flatten().ElementAt(0));
            Assert.AreEqual(ClrType<string>(), ((SumTerm)actual).Flatten().ElementAt(1));
        }

        [Test]
        public void SumToWidenTypeTerm()
        {
            // System.IFormattable :> (System.Int32:* + System.Double:*)
            // --> System.IFormattable:*
            var term =
                WideningClrType(
                    Identity("System.IFormattable"),
                    Sum(
                        Identity("System.Int32"),
                        Identity("System.Double")));

            var environment = ClrEnvironmentFactory.Create();

            var actual = environment.Reduce(term);

            Assert.AreEqual(ClrType<IFormattable>(), actual);
        }

        [Test]
        public void ProductTypeTerm()
        {
            // let combined = System.Int32:* * System.String:*
            var term =
                BindMutable(
                    "combined",
                    Product(
                        Identity("System.Int32"),
                        Identity("System.String")));

            var environment = ClrEnvironmentFactory.Create();

            var actual = environment.Reduce(term);

            Assert.AreEqual(ClrType<int>(), ((ProductTerm)actual).Flatten().ElementAt(0));
            Assert.AreEqual(ClrType<string>(), ((ProductTerm)actual).Flatten().ElementAt(1));
        }

        private sealed class _1 { }
        private sealed class _2 { }

        // int: int <== int
        [TestCase(new[] { typeof(int) }, new[] { typeof(int) }, new[] { typeof(int) })]
        // IComparable: IComparable <== IComparable
        [TestCase(new[] { typeof(IComparable) }, new[] { typeof(IComparable) }, new[] { typeof(IComparable) })]
        // _[1]: _[1] <== _[1]
        [TestCase(new[] { typeof(_1) }, new[] { typeof(_1) }, new[] { typeof(_1) })]
        // object: object <== int
        [TestCase(new[] { typeof(object) }, new[] { typeof(object) }, new[] { typeof(int) })]
        // IComparable: IComparable <== string
        [TestCase(new[] { typeof(IComparable) }, new[] { typeof(IComparable) }, new[] { typeof(string) })]
        // _: _ <== int
        [TestCase(new[] { typeof(_1) }, new[] { typeof(_1) }, new[] { typeof(int) })]
        // _: _ <== (int + double)
        [TestCase(new[] { typeof(_1) }, new[] { typeof(_1) }, new[] { typeof(int), typeof(double) })]
        // _[1]: _[1] <== _[2]
        //[TestCase(new[] { typeof(_1) }, new[] { typeof(_1) }, new[] { typeof(_2) })]
        // (int + _): (int + _) <== string
        [TestCase(new[] { typeof(int), typeof(_1) }, new[] { typeof(int), typeof(_1) }, new[] { typeof(string) })]
        // (int + _): (int + _) <== (int + string)
        [TestCase(new[] { typeof(int), typeof(_1) }, new[] { typeof(int), typeof(_1) }, new[] { typeof(int), typeof(string) })]
        // (int + _[1]): (int + _[1]) <== _[2]
        [TestCase(new[] { typeof(int), typeof(_1) }, new[] { typeof(int), typeof(_1) }, new[] { typeof(_2) })]
        // (_[1] + _[2]): (_[1] + _[2]) <== (_[2] + _[1])
        [TestCase(new[] { typeof(_1), typeof(_2) }, new[] { typeof(_1), typeof(_2) }, new[] { typeof(_2), typeof(_1) })]
        // (int + double): (int + double) <== (int + double)
        [TestCase(new[] { typeof(int), typeof(double) }, new[] { typeof(int), typeof(double) }, new[] { typeof(int), typeof(double) })]
        // (int + double + string): (int + double + string) <== (int + double)
        [TestCase(new[] { typeof(int), typeof(double), typeof(string) }, new[] { typeof(int), typeof(double), typeof(string) }, new[] { typeof(int), typeof(double) })]
        // (int + IComparable): (int + IComparable) <== (int + string)
        [TestCase(new[] { typeof(int), typeof(IComparable) }, new[] { typeof(int), typeof(IComparable) }, new[] { typeof(int), typeof(string) })]
        // null: int <== (int + double)
        [TestCase(new Type[0], new[] { typeof(int) }, new[] { typeof(int), typeof(double) })]
        // null: (int + double) <== (int + double + string)
        [TestCase(new Type[0], new[] { typeof(int), typeof(double) }, new[] { typeof(int), typeof(double), typeof(string) })]
        // null: (int + IServiceProvider) <== (int + double)
        [TestCase(new Type[0], new[] { typeof(int), typeof(IServiceProvider) }, new[] { typeof(int), typeof(double) })]
        // null: int <== _   [TODO: maybe]
        [TestCase(new Type[0], new[] { typeof(int) }, new[] { typeof(_1) })]
        // (int + double): (int + double) <== int
        [TestCase(new[] { typeof(int), typeof(double) }, new[] { typeof(int), typeof(double) }, new[] { typeof(int) })]
        // (int + IServiceProvider): (int + IServiceProvider) <== int
        [TestCase(new[] { typeof(int), typeof(IServiceProvider) }, new[] { typeof(int), typeof(IServiceProvider) }, new[] { typeof(int) })]
        // (int + IComparable): (int + IComparable) <== string
        [TestCase(new[] { typeof(int), typeof(IComparable) }, new[] { typeof(int), typeof(IComparable) }, new[] { typeof(string) })]
        public void TypeWidening(Type[] expectedTypes, Type[] toTypes, Type[] fromTypes)
        {
            Assert.IsTrue(toTypes.Length >= 1);
            Assert.IsTrue(fromTypes.Length >= 1);

            var environment = ClrEnvironmentFactory.Create();
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

            var to = Sum(toTypes.Select(CreateTermFromType))!;
            var from = Sum(fromTypes.Select(CreateTermFromType))!;

            var actual = ClrTypeCalculator.Instance.Widening(to, from);

            var expected = Sum(expectedTypes.Select(CreateTermFromType));

            Assert.AreEqual(expected, actual);
        }

        // int->int: int->int <== int->int
        [TestCase(new[] { typeof(Func<int, int>) }, new[] { typeof(Func<int, int>) }, new[] { typeof(Func<int, int>) })]
        // int->double: int->double <== int->double
        [TestCase(new[] { typeof(Func<int, double>) }, new[] { typeof(Func<int, double>) }, new[] { typeof(Func<int, double>) })]
        // int->double: int->double <== object->double
        [TestCase(new[] { typeof(Func<int, double>) }, new[] { typeof(Func<int, double>) }, new[] { typeof(Func<object, double>) })]
        // int->object: int->object <== int->double
        [TestCase(new[] { typeof(Func<int, object>) }, new[] { typeof(Func<int, object>) }, new[] { typeof(Func<int, double>) })]
        public void LambdaTypeWidening(Type[] expectedTypes, Type[] toTypes, Type[] fromTypes)
        {
            Assert.IsTrue(toTypes.Length >= 1);
            Assert.IsTrue(fromTypes.Length >= 1);

            var environment = ClrEnvironmentFactory.Create();
            var p1 = environment.CreatePlaceholder(Unspecified());
            var p2 = environment.CreatePlaceholder(Unspecified());

            Term CreateTermFromType(Type type)
            {
                if (type.IsGenericType && !type.IsGenericTypeDefinition &&
                    type.GetGenericTypeDefinition() == typeof(Func<,>))
                {
                    var args = type.GetGenericArguments();
                    return Lambda(CreateTermFromType(args[0]), CreateTermFromType(args[1]));
                }

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

            var to = Sum(toTypes.Select(CreateTermFromType))!;
            var from = Sum(fromTypes.Select(CreateTermFromType))!;

            var actual = ClrTypeCalculator.Instance.Widening(to, from);

            var expected = Sum(expectedTypes.Select(CreateTermFromType));

            Assert.AreEqual(expected, actual);
        }

        // decimal <== double
        [TestCase(typeof(decimal), typeof(double))]
        // decimal <== float
        [TestCase(typeof(decimal), typeof(float))]
        // decimal <== long
        [TestCase(typeof(decimal), typeof(long))]
        // decimal <== ulong
        [TestCase(typeof(decimal), typeof(ulong))]
        // decimal <== int
        [TestCase(typeof(decimal), typeof(int))]
        // decimal <== uint
        [TestCase(typeof(decimal), typeof(uint))]
        // decimal <== short
        [TestCase(typeof(decimal), typeof(short))]
        // decimal <== ushort
        [TestCase(typeof(decimal), typeof(ushort))]
        // decimal <== byte
        [TestCase(typeof(decimal), typeof(byte))]
        // decimal <== sbyte
        [TestCase(typeof(decimal), typeof(sbyte))]
        // decimal <== char
        [TestCase(typeof(decimal), typeof(char))]

        // double <== float
        [TestCase(typeof(double), typeof(float))]
        // double <== int
        [TestCase(typeof(double), typeof(int))]
        // double <== uint
        [TestCase(typeof(double), typeof(uint))]
        // double <== short
        [TestCase(typeof(double), typeof(short))]
        // double <== ushort
        [TestCase(typeof(double), typeof(ushort))]
        // double <== byte
        [TestCase(typeof(double), typeof(byte))]
        // double <== sbyte
        [TestCase(typeof(double), typeof(sbyte))]
        // double <== char
        [TestCase(typeof(double), typeof(char))]

        // float <== int
        [TestCase(typeof(float), typeof(int))]
        // float <== uint
        [TestCase(typeof(float), typeof(uint))]
        // float <== short
        [TestCase(typeof(float), typeof(short))]
        // float <== ushort
        [TestCase(typeof(float), typeof(ushort))]
        // float <== byte
        [TestCase(typeof(float), typeof(byte))]
        // float <== sbyte
        [TestCase(typeof(float), typeof(sbyte))]
        // float <== char
        [TestCase(typeof(float), typeof(char))]

        // long <== int
        [TestCase(typeof(long), typeof(int))]
        // long <== uint
        [TestCase(typeof(long), typeof(uint))]
        // long <== short
        [TestCase(typeof(long), typeof(short))]
        // long <== ushort
        [TestCase(typeof(long), typeof(ushort))]
        // long <== byte
        [TestCase(typeof(long), typeof(byte))]
        // long <== sbyte
        [TestCase(typeof(long), typeof(sbyte))]
        // long <== char
        [TestCase(typeof(long), typeof(char))]

        // ulong <== uint
        [TestCase(typeof(ulong), typeof(uint))]
        // ulong <== ushort
        [TestCase(typeof(ulong), typeof(ushort))]
        // ulong <== byte
        [TestCase(typeof(ulong), typeof(byte))]
        // ulong <== char
        [TestCase(typeof(ulong), typeof(char))]

        // int <== short
        [TestCase(typeof(int), typeof(short))]
        // int <== ushort
        [TestCase(typeof(int), typeof(ushort))]
        // int <== byte
        [TestCase(typeof(int), typeof(byte))]
        // int <== sbyte
        [TestCase(typeof(int), typeof(sbyte))]
        // int <== char
        [TestCase(typeof(int), typeof(char))]

        // uint <== ushort
        [TestCase(typeof(uint), typeof(ushort))]
        // uint <== byte
        [TestCase(typeof(uint), typeof(byte))]
        // uint <== char
        [TestCase(typeof(uint), typeof(char))]

        // short <== byte
        [TestCase(typeof(short), typeof(byte))]
        // short <== sbyte
        [TestCase(typeof(short), typeof(sbyte))]

        // ushort <== byte
        [TestCase(typeof(ushort), typeof(byte))]

        // char <== byte
        [TestCase(typeof(char), typeof(byte))]
        public void PrimitiveTypeWidening(Type toType, Type fromType)
        {
            var environment = ClrEnvironmentFactory.Create();

            var to = Constant(toType);
            var from = Constant(fromType);

            var actual1 = ClrTypeCalculator.Instance.Widening(to, from);

            Assert.AreEqual(to, actual1);

            var actual2 = ClrTypeCalculator.Instance.Widening(from, to);

            Assert.IsNull(actual2);
        }

        [Test]
        public void CharTypeWidening()
        {
            // Both are correct:
            // ushort <== char
            // char <== ushort
            var environment = ClrEnvironmentFactory.Create();

            var to = Constant(typeof(char));
            var from = Constant(typeof(ushort));

            var actual1 = ClrTypeCalculator.Instance.Widening(to, from);

            Assert.AreEqual(to, actual1);

            var actual2 = ClrTypeCalculator.Instance.Widening(from, to);

            Assert.AreEqual(from, actual2);
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
