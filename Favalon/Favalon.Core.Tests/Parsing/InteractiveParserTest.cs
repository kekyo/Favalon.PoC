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
using System.Threading.Tasks;

namespace Favalon.Parsing
{
    [TestFixture]
    public sealed class InteractiveParserTest
    {
        [Test]
        public Task ApplyVariables()
        {
            var host = InteractiveTestHost.Create("aaa bbb ccc");
            var parser = InteractiveParser.Create(host);

            return host.RunAsync(async () =>
            {
                var result = await parser.Awaitable;
                Assert.AreEqual("((aaa:_ bbb:_):_ ccc:_):_", result.Term.StrictReadableString);
            });
        }

        [Test]
        public Task ApplyCombinedVariableAndNumeric()
        {
            var host = InteractiveTestHost.Create("aaa 123");
            var parser = InteractiveParser.Create(host);

            return host.RunAsync(async () =>
            {
                var result = await parser.Awaitable;
                Assert.AreEqual("(aaa:_ 123:?):_", result.Term.StrictReadableString);
            });
        }

        [Test]
        public Task ApplyCombinedVariableAndSymbol()
        {
            var host = InteractiveTestHost.Create("aaa | bbb");
            var parser = InteractiveParser.Create(host);

            return host.RunAsync(async () =>
            {
                var result = await parser.Awaitable;
                Assert.AreEqual("((aaa:_ |:_):_ bbb:_):_", result.Term.StrictReadableString);
            });
        }
    }
}
