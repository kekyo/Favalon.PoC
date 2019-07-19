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

namespace Favalon
{
    [TestFixture]
    public sealed class ParserTest
    {
        [Test]
        public void ApplyVariables()
        {
            var parser = Parser.Create();
            var (term, errors) = parser.Append("aaa bbb ccc", 0);
            Assert.AreEqual("((aaa:_ bbb:_):_ ccc:_):_", term?.StrictReadableString);
            Assert.AreEqual(0, errors.Length);
        }

        [Test]
        public void ApplyCombinedVariableAndNumeric()
        {
            var parser = Parser.Create();
            var (term, errors) = parser.Append("aaa 123", 0);
            Assert.AreEqual("(aaa:_ 123:?):_", term?.StrictReadableString);
            Assert.AreEqual(0, errors.Length);
        }

        [Test]
        public void ApplyCombinedVariableAndSymbol()
        {
            var parser = Parser.Create();
            var (term, errors) = parser.Append("aaa | bbb", 0);
            Assert.AreEqual("((aaa:_ |:_):_ bbb:_):_", term?.StrictReadableString);
            Assert.AreEqual(0, errors.Length);
        }
    }
}
