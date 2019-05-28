using NUnit.Framework;
using System.Linq;

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
            Assert.AreEqual(args[resultIndex], actual);
        }

        [TestCase(0, 1)]
        [TestCase(1, 0)]
        public void BothStringAndInt32WideIsUnion(int index0, int index1)
        {
            var args = new[] { StringType.Instance, Int32Type.Instance };

            var actual = AvalonType.Wide(args[index0], args[index1]).
                EnumerateTypes().
                ToArray();
            Assert.IsFalse(new[] { StringType.Instance, Int32Type.Instance }.Except(actual).Any());
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
            Assert.IsFalse(new[] { StringType.Instance, DoubleType.Instance }.Except(actual).Any());
        }
    }
}
