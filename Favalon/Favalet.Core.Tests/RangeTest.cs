// This is part of Favalon project - https://github.com/kekyo/Favalon
// Copyright (c) 2019 Kouji Matsui
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using NUnit.Framework;

namespace Favalet
{
    [TestFixture]
    public sealed class RangeTest
    {
        [Test]
        public void ToString1()
        {
            var range = Range.Create(Position.Create(1, 2), Position.Create(3, 4));

            Assert.AreEqual("1,2,3,4", range.ToString());
        }

        [Test]
        public void ToString2()
        {
            var range = Range.Create(Position.Create(1, 2));

            Assert.AreEqual("1,2", range.ToString());
        }

        [Test]
        public void Contains1()
        {
            var outer = Range.Create(Position.Create(1, 1), Position.Create(10, 10));
            var inner = Range.Create(Position.Create(5, 5));

            Assert.IsTrue(outer.Contains(inner));
        }

        [Test]
        public void Contains2()
        {
            var outer = Range.Create(Position.Create(1, 4), Position.Create(10, 6));
            var inner = Range.Create(Position.Create(2, 2));

            Assert.IsTrue(outer.Contains(inner));
        }

        [Test]
        public void Contains3()
        {
            var outer = Range.Create(Position.Create(1, 4), Position.Create(10, 6));
            var inner = Range.Create(Position.Create(9, 8));

            Assert.IsTrue(outer.Contains(inner));
        }

        [Test]
        public void Contains4()
        {
            var outer = Range.Create(Position.Create(1, 4), Position.Create(10, 6));
            var inner = Range.Create(Position.Create(1, 4));

            Assert.IsTrue(outer.Contains(inner));
        }

        [Test]
        public void Contains5()
        {
            var outer = Range.Create(Position.Create(1, 4), Position.Create(10, 6));
            var inner = Range.Create(Position.Create(10, 6));

            Assert.IsTrue(outer.Contains(inner));
        }

        [Test]
        public void Contains6()
        {
            var outer = Range.Create(Position.Create(1, 4), Position.Create(10, 6));
            var inner = Range.Create(Position.Create(1, 3));

            Assert.IsFalse(outer.Contains(inner));
        }

        [Test]
        public void Contains7()
        {
            var outer = Range.Create(Position.Create(1, 4), Position.Create(10, 6));
            var inner = Range.Create(Position.Create(10, 7));

            Assert.IsFalse(outer.Contains(inner));
        }

        [Test]
        public void Contains8()
        {
            var outer = Range.Create(Position.Create(1, 4), Position.Create(10, 6));
            var inner = Range.Create(Position.Create(1, 3), Position.Create(1, 4));

            Assert.IsFalse(outer.Contains(inner));
        }

        [Test]
        public void Contains9()
        {
            var outer = Range.Create(Position.Create(1, 4), Position.Create(10, 6));
            var inner = Range.Create(Position.Create(10, 6), Position.Create(10, 7));

            Assert.IsFalse(outer.Contains(inner));
        }

        [Test]
        public void Contains10()
        {
            var outer = Range.Create(Position.Create(1, 4), Position.Create(10, 6));
            var inner = Range.Create(Position.Create(1, 4), Position.Create(1, 5));

            Assert.IsTrue(outer.Contains(inner));
        }

        [Test]
        public void Contains11()
        {
            var outer = Range.Create(Position.Create(1, 4), Position.Create(10, 6));
            var inner = Range.Create(Position.Create(10, 5), Position.Create(10, 6));

            Assert.IsTrue(outer.Contains(inner));
        }

        [Test]
        public void Contains12()
        {
            var outer = Range.Create(Position.Create(1, 4), Position.Create(10, 6));
            var inner = Range.Create(Position.Create(2, 1), Position.Create(9, 11));

            Assert.IsTrue(outer.Contains(inner));
        }

        [Test]
        public void Contains13()
        {
            var outer = Range.Create(Position.Create(1, 4), Position.Create(10, 6));
            var inner = Range.Create(Position.Create(1, 4), Position.Create(10, 6));

            Assert.IsTrue(outer.Contains(inner));
        }

        [Test]
        public void Tuple1()
        {
            Range range = (1, 2);

            Assert.AreEqual("1,2", range.ToString());
        }

        [Test]
        public void Tuple2()
        {
            Range range = (1, 2, 3, 4);

            Assert.AreEqual("1,2,3,4", range.ToString());
        }
    }
}
