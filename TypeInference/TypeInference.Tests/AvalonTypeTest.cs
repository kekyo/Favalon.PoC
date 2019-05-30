using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using TypeInferences.Types;

namespace TypeInferences
{
    [TestFixture]
    public sealed class AvalonTypeTest
    {
        [TestCase(0, 0, 0)]
        [TestCase(0, 0, 1)]
        [TestCase(0, 1, 0)]
        [TestCase(1, 1, 1)]
        [TestCase(0, 0, 2)]
        [TestCase(0, 2, 0)]
        [TestCase(2, 2, 2)]
        [TestCase(1, 1, 2)]
        [TestCase(1, 2, 1)]
        public void BothInt32AndInt32Wide(int resultIndex,  int index0, int index1)
        {
            var args = new[] { DoubleType.Instance, Int32Type.Instance, UInt16Type.Instance };

            var actual = AvalonType.Wide(args[index0], args[index1]);

            var expected = args[resultIndex];
            Assert.AreEqual(expected, actual);
        }

        private sealed class ObjectComparer<T> : IComparer<T>
        {
            public int Compare(T x, T y) =>
                 x!.GetHashCode().CompareTo(y!.GetHashCode());

            public static readonly IComparer<T> Instance = new ObjectComparer<T>();
        }

        private static bool SequenceEqualNotOrdered<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            var fex = expected.OrderBy(v => v, ObjectComparer<T>.Instance);
            var fac = actual.OrderBy(v => v, ObjectComparer<T>.Instance);
            return fex.SequenceEqual(fac);
        }

        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void BothStringAndInt32WideIsUnion(int index0, int index1)
        {
            var args = new[] { StringType.Instance, Int32Type.Instance };

            var actual = AvalonType.Wide(args[index0], args[index1]).
                EnumerateTypes().
                ToArray();

            var expected = new[] { StringType.Instance, Int32Type.Instance };
            Assert.IsTrue(SequenceEqualNotOrdered(expected, actual));
        }

        [TestCase(0, 1, 2)]
        [TestCase(1, 0, 2)]
        [TestCase(0, 2, 1)]
        [TestCase(1, 2, 0)]
        [TestCase(2, 0, 1)]
        [TestCase(2, 1, 0)]
        public void BothDoubleStringInt32WideIsUnion(int index0, int index1, int index2)
        {
            var args = new[] { DoubleType.Instance, StringType.Instance, Int32Type.Instance };

            var actual = AvalonType.Wide(args[index0], args[index1], args[index2]).
                EnumerateTypes().
                ToArray();

            var expected = new[] { StringType.Instance, DoubleType.Instance };
            Assert.IsTrue(SequenceEqualNotOrdered(expected, actual));
        }

        [TestCase(3, 0, 1, 2)]
        [TestCase(3, 1, 0, 2)]
        [TestCase(3, 0, 2, 1)]
        [TestCase(3, 1, 2, 0)]
        [TestCase(3, 2, 0, 1)]
        [TestCase(3, 2, 1, 0)]
        [TestCase(0, 3, 1, 2)]
        [TestCase(1, 3, 0, 2)]
        [TestCase(0, 3, 2, 1)]
        [TestCase(1, 3, 2, 0)]
        [TestCase(2, 3, 0, 1)]
        [TestCase(2, 3, 1, 0)]
        [TestCase(0, 1, 3, 2)]
        [TestCase(1, 0, 3, 2)]
        [TestCase(0, 2, 3, 1)]
        [TestCase(1, 2, 3, 0)]
        [TestCase(2, 0, 3, 1)]
        [TestCase(2, 1, 3, 0)]
        [TestCase(0, 1, 2, 3)]
        [TestCase(1, 0, 2, 3)]
        [TestCase(0, 2, 1, 3)]
        [TestCase(1, 2, 0, 3)]
        [TestCase(2, 0, 1, 3)]
        [TestCase(2, 1, 0, 3)]
        public void BothObjectStringInt32WideIsUnion(int index0, int index1, int index2, int index3)
        {
            var args = new[] { DoubleType.Instance, StringType.Instance, Int32Type.Instance, ObjectType.Instance };

            var actual = AvalonType.Wide(args[index0], args[index1], args[index2], args[index3]).
                EnumerateTypes().
                ToArray();

            var expected = new[] { ObjectType.Instance };
            Assert.IsTrue(SequenceEqualNotOrdered(expected, actual));
        }
    }
}
