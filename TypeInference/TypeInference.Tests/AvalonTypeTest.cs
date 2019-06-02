using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TypeInferences.Types
{
    [TestFixture]
    public sealed class AvalonTypeTest
    {
        private static readonly AvalonType doubleType = AvalonType.FromClrType<double>();
        private static readonly AvalonType int32Type = AvalonType.FromClrType<int>();
        private static readonly AvalonType uint16Type = AvalonType.FromClrType<ushort>();
        private static readonly AvalonType stringType = AvalonType.FromClrType<string>();
        private static readonly AvalonType objectType = AvalonType.FromClrType<object>();

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
            var args = new[] { doubleType, int32Type, uint16Type };

            var arg0 = args[index0].MakeTypeRef();
            arg0.ComposeToWide(args[index1]);

            var expected = args[resultIndex];
            Assert.AreEqual(expected, arg0);
        }

        private static bool SequenceEqualNotOrdered<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            var fex = expected.OrderBy(v => v);
            var fac = actual.OrderBy(v => v);
            return fex.SequenceEqual(fac);
        }

        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void BothStringAndInt32WideIsUnion(int index0, int index1)
        {
            var args = new[] { stringType, int32Type };

            var arg0 = args[index0].MakeTypeRef();
            arg0.ComposeToWide(args[index1]);

            var actual = arg0.
                EnumerateTypes().
                ToArray();

            var expected = new[] { stringType, int32Type };
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
            var args = new[] { doubleType, stringType, int32Type };

            var arg0 = args[index0].MakeTypeRef();
            arg0.ComposeToWide(args[index1], args[index2]);

            var actual = arg0.
                EnumerateTypes().
                ToArray();

            var expected = new[] { stringType, doubleType };
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
            var args = new[] { doubleType, stringType, int32Type, objectType };

            var arg0 = args[index0].MakeTypeRef();
            arg0.ComposeToWide(args[index1], args[index2], args[index3]);

            var actual = arg0.
                EnumerateTypes().
                ToArray();

            var expected = new[] { objectType };
            Assert.IsTrue(SequenceEqualNotOrdered(expected, actual));
        }
    }
}
