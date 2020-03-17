////////////////////////////////////////////////////////////////////////////
//
// Favalon - An Interactive Shell Based on a Typed Lambda Calculus.
// Copyright (c) 2018-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using Favalet.Lexers;
using Favalet.Tokens;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using static Favalet.Tokens.TokenFactory;

namespace Favalet
{
    [TestFixture]
    public sealed class LexerTest
    {
        private static readonly Func<string, ValueTask<Token[]>>[] Tokenizers =
            new[]
            {
                new Func<string, ValueTask<Token[]>>(text => new ValueTask<Token[]>(Lexer.Tokenize(text).ToArray())),
                new Func<string, ValueTask<Token[]>>(text => new ValueTask<Token[]>(Lexer.Tokenize(text.AsEnumerable()).ToArray())),
                new Func<string, ValueTask<Token[]>>(text => new ValueTask<Token[]>(Lexer.Tokenize(new StringReader(text)).ToArray())),
                new Func<string, ValueTask<Token[]>>(async text => await Lexer.Tokenize(text.ToObservable()).ToArray()),
            };

        ////////////////////////////////////////////////////

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableIdentityTokens(Func<string, ValueTask<Token[]>> run)
        {
            var text = "abc def ghi";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    WhiteSpace(),
                    Identity("def"),
                    WhiteSpace(),
                    Identity("ghi") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableIdentityTokensBeforeSpace(Func<string, ValueTask<Token[]>> run)
        {
            var text = "  abc def ghi";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    WhiteSpace(),
                    Identity("abc"),
                    WhiteSpace(),
                    Identity("def"),
                    WhiteSpace(),
                    Identity("ghi") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableIdentityTokensAfterSpace(Func<string, ValueTask<Token[]>> run)
        {
            var text = "abc def ghi  ";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    WhiteSpace(),
                    Identity("def"),
                    WhiteSpace(),
                    Identity("ghi"),
                    WhiteSpace() },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableIdentityTokensLongSpace(Func<string, ValueTask<Token[]>> run)
        {
            var text = "abc      def      ghi";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    WhiteSpace(),
                    Identity("def"),
                    WhiteSpace(),
                    Identity("ghi") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableIdentityTokensBeforeBrackets(Func<string, ValueTask<Token[]>> run)
        {
            var text = "(abc def) ghi";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Open('('),
                    Identity("abc"),
                    WhiteSpace(),
                    Identity("def"),
                    Close(')'),
                    WhiteSpace(),
                    Identity("ghi") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableIdentityTokensAfterBrackets(Func<string, ValueTask<Token[]>> run)
        {
            var text = "abc (def ghi)";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    WhiteSpace(),
                    Open('('),
                    Identity("def"),
                    WhiteSpace(),
                    Identity("ghi"),
                    Close(')') },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableIdentityTokensWithSpacingBrackets(Func<string, ValueTask<Token[]>> run)
        {
            var text = "abc ( def ) ghi";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    WhiteSpace(),
                    Open('('),
                    WhiteSpace(),
                    Identity("def"),
                    WhiteSpace(),
                    Close(')'),
                    WhiteSpace(),
                    Identity("ghi") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableIdentityTokensWithNoSpacingBrackets(Func<string, ValueTask<Token[]>> run)
        {
            var text = "abc(def)ghi";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    Open('('),
                    Identity("def"),
                    Close(')'),
                    Identity("ghi") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableIdentityTrailsNumericTokens(Func<string, ValueTask<Token[]>> run)
        {
            var text = "a12 d34 g56";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("a12"),
                    WhiteSpace(),
                    Identity("d34"),
                    WhiteSpace(),
                    Identity("g56") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableSignLikeOperatorAndIdentityTokens1(Func<string, ValueTask<Token[]>> run)
        {
            var text = "+abc";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("+"),
                    Identity("abc") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableSignLikeOperatorAndIdentityTokens2(Func<string, ValueTask<Token[]>> run)
        {
            var text = "-abc";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("-"),
                    Identity("abc") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableStrictOperatorAndIdentityTokens1(Func<string, ValueTask<Token[]>> run)
        {
            var text = "++abc";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("++"),
                    Identity("abc") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableStrictOperatorAndIdentityTokens2(Func<string, ValueTask<Token[]>> run)
        {
            var text = "--abc";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("--"),
                    Identity("abc") },
                actual);
        }

        ///////////////////////////////////////////////

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableNumericTokens(Func<string, ValueTask<Token[]>> run)
        {
            var text = "123 456 789";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    WhiteSpace(),
                    Numeric("456"),
                    WhiteSpace(),
                    Numeric("789") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableCombinedIdentityAndNumericTokens(Func<string, ValueTask<Token[]>> run)
        {
            var text = "abc 456 def";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    WhiteSpace(),
                    Numeric("456"),
                    WhiteSpace(),
                    Identity("def") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableNumericTokensBeforeBrackets(Func<string, ValueTask<Token[]>> run)
        {
            var text = "(123 456) 789";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Open('('),
                    Numeric("123"),
                    WhiteSpace(),
                    Numeric("456"),
                    Close(')'),
                    WhiteSpace(),
                    Numeric("789") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableNumericTokensAfterBrackets(Func<string, ValueTask<Token[]>> run)
        {
            var text = "123 (456 789)";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    WhiteSpace(),
                    Open('('),
                    Numeric("456"),
                    WhiteSpace(),
                    Numeric("789"),
                    Close(')') },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableNumericTokensWithSpacingBrackets(Func<string, ValueTask<Token[]>> run)
        {
            var text = "123 ( 456 ) 789";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    WhiteSpace(),
                    Open('('),
                    WhiteSpace(),
                    Numeric("456"),
                    WhiteSpace(),
                    Close(')'),
                    WhiteSpace(),
                    Numeric("789") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableNumericTokensWithNoSpacingBrackets(Func<string, ValueTask<Token[]>> run)
        {
            var text = "123(456)789";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    Open('('),
                    Numeric("456"),
                    Close(')'),
                    Numeric("789") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableStringToken(Func<string, ValueTask<Token[]>> run)
        {
            var text = "\"abc def\"";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc def") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableStringTokenBeforeSpacing(Func<string, ValueTask<Token[]>> run)
        {
            var text = " \"abc def\"";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    WhiteSpace(),
                    String("abc def") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableStringTokenAfterSpacing(Func<string, ValueTask<Token[]>> run)
        {
            var text = "\"abc def\" ";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc def"),
                    WhiteSpace() },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableStringTokenBothSpacing(Func<string, ValueTask<Token[]>> run)
        {
            var text = " \"abc def\" ";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    WhiteSpace(),
                    String("abc def"),
                    WhiteSpace() },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableNumericTokensBeforeString(Func<string, ValueTask<Token[]>> run)
        {
            var text = "\"abc def\" 123";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc def"),
                    WhiteSpace(),
                    Numeric("123") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableNumericTokensAfterString(Func<string, ValueTask<Token[]>> run)
        {
            var text = "123 \"abc def\"";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    WhiteSpace(),
                    String("abc def") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableNumericTokensSpacingStrings(Func<string, ValueTask<Token[]>> run)
        {
            var text = "123 \"abc def\" 456";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    WhiteSpace(),
                    String("abc def"),
                    WhiteSpace(),
                    Numeric("456") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableNumericTokensNoSpacingStrings(Func<string, ValueTask<Token[]>> run)
        {
            var text = "123\"abc def\"456";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    String("abc def"),
                    Numeric("456") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableIdentityTokensBeforeStringWithNoSpacing(Func<string, ValueTask<Token[]>> run)
        {
            var text = "\"abc def\"abc";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc def"),
                    Identity("abc") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableIdentityTokensAfterStringWithNoSpacing(Func<string, ValueTask<Token[]>> run)
        {
            var text = "abc\"abc def\"";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    String("abc def") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableIdentityTokensNoSpacingStrings(Func<string, ValueTask<Token[]>> run)
        {
            var text = "abc\"abc def\"def";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    String("abc def"),
                    Identity("def") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableStringTokensNoSpacingOperators(Func<string, ValueTask<Token[]>> run)
        {
            var text = "+\"abc def\"-";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("+"),
                    String("abc def"),
                    Identity("-") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableOperatorTokensNoSpacingStrings(Func<string, ValueTask<Token[]>> run)
        {
            var text = "\"abc\"+\"def\"";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc"),
                    Identity("+"),
                    String("def") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableStringTokensNoSpacingBrackets(Func<string, ValueTask<Token[]>> run)
        {
            var text = "(\"abc def\")";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Open('('),
                    String("abc def"),
                    Close(')') },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableOpenBracketTokensNoSpacingStrings(Func<string, ValueTask<Token[]>> run)
        {
            var text = "\"abc\"(\"def\"";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc"),
                    Open('('),
                    String("def") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableCloseBracketTokensNoSpacingStrings(Func<string, ValueTask<Token[]>> run)
        {
            var text = "\"abc\")\"def\"";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc"),
                    Close(')'),
                    String("def") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableContinuousStrings(Func<string, ValueTask<Token[]>> run)
        {
            var text = "\"abc\"\"def\"";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc"),
                    String("def") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerablePlusSignNumericTokens(Func<string, ValueTask<Token[]>> run)
        {
            var text = "+123 +456 +789";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    PlusSign(),
                    Numeric("123"),
                    WhiteSpace(),
                    PlusSign(),
                    Numeric("456"),
                    WhiteSpace(),
                    PlusSign(),
                    Numeric("789") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableMinusSignNumericTokens(Func<string, ValueTask<Token[]>> run)
        {
            var text = "-123 -456 -789";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    MinusSign(),
                    Numeric("123"),
                    WhiteSpace(),
                    MinusSign(),
                    Numeric("456"),
                    WhiteSpace(),
                    MinusSign(),
                    Numeric("789") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerablePlusOperatorNumericTokens(Func<string, ValueTask<Token[]>> run)
        {
            var text = "+ 123 + 456 + 789";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("+"),
                    WhiteSpace(),
                    Numeric("123"),
                    WhiteSpace(),
                    Identity("+"),
                    WhiteSpace(),
                    Numeric("456"),
                    WhiteSpace(),
                    Identity("+"),
                    WhiteSpace(),
                    Numeric("789") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableMinusOperatorNumericTokens(Func<string, ValueTask<Token[]>> run)
        {
            var text = "- 123 - 456 - 789";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("-"),
                    WhiteSpace(),
                    Numeric("123"),
                    WhiteSpace(),
                    Identity("-"),
                    WhiteSpace(),
                    Numeric("456"),
                    WhiteSpace(),
                    Identity("-"),
                    WhiteSpace(),
                    Numeric("789") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerablePlusOperatorWithSpaceAndNumericTokens(Func<string, ValueTask<Token[]>> run)
        {
            var text = "123 + 456";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    WhiteSpace(),
                    Identity("+"),
                    WhiteSpace(),
                    Numeric("456") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableMinusOperatorWithSpaceAndNumericTokens(Func<string, ValueTask<Token[]>> run)
        {
            var text = "123 - 456";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    WhiteSpace(),
                    Identity("-"),
                    WhiteSpace(),
                    Numeric("456") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerablePlusOperatorSideBySideAndNumericTokens2(Func<string, ValueTask<Token[]>> run)
        {
            var text = "123+456";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    PlusSign(),
                    Numeric("456") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableMinusOperatorSideBySideAndNumericTokens(Func<string, ValueTask<Token[]>> run)
        {
            var text = "123-456";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    MinusSign(),
                    Numeric("456") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableComplexNumericOperatorTokens1(Func<string, ValueTask<Token[]>> run)
        {
            var text = "-123*(+456+789)";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    MinusSign(),
                    Numeric("123"),
                    Identity("*"),
                    Open('('),
                    PlusSign(),
                    Numeric("456"),
                    PlusSign(),
                    Numeric("789"),
                    Close(')')
                },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableComplexNumericOperatorTokens2(Func<string, ValueTask<Token[]>> run)
        {
            var text = "+123*(-456-789)";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    PlusSign(),
                    Numeric("123"),
                    Identity("*"),
                    Open('('),
                    MinusSign(),
                    Numeric("456"),
                    MinusSign(),
                    Numeric("789"),
                    Close(')')
                },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableStrictOperatorAndNumericTokens1(Func<string, ValueTask<Token[]>> run)
        {
            var text = "++123";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("++"),
                    Numeric("123") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableStrictOperatorAndNumericTokens2(Func<string, ValueTask<Token[]>> run)
        {
            var text = "--123";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("--"),
                    Numeric("123") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableCombineIdentityAndNumericTokensWithOperator(Func<string, ValueTask<Token[]>> run)
        {
            var text = "abc+123";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    PlusSign(),
                    Numeric("123") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task EnumerableCombineNumericAndIdentityTokensWithOperator(Func<string, ValueTask<Token[]>> run)
        {
            var text = "123+abc";
            var actual = await run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    Identity("+"),
                    Identity("abc") },
                actual);
        }

        [TestCaseSource("Tokenizers")]
        public async Task Operator1Detection(Func<string, ValueTask<Token[]>> run)
        {
            foreach (var ch in TokenFactory.OperatorChars)
            {
                var text = $"123 {ch} abc";
                var actual = await run(text);

                Assert.AreEqual(
                    new Token[] {
                        Numeric("123"),
                        WhiteSpace(),
                        Identity(ch.ToString()),
                        WhiteSpace(),
                        Identity("abc") },
                    actual);
            }
        }

        [TestCaseSource("Tokenizers")]
        public async Task Operator2Detection(Func<string, ValueTask<Token[]>> run)
        {
            await Task.WhenAll(
                TokenFactory.OperatorChars.
                    SelectMany(ch1 => TokenFactory.OperatorChars.
                        Select(ch2 => (ch1, ch2))).
                Select(async entry =>
                {
                    var text = $"123 {entry.ch1}{entry.ch2} abc";
                    var actual = await run(text);

                    Assert.AreEqual(
                        new Token[] {
                            Numeric("123"),
                            WhiteSpace(),
                            Identity($"{entry.ch1}{entry.ch2}"),
                            WhiteSpace(),
                            Identity("abc") },
                        actual);
                }));
        }

        [TestCaseSource("Tokenizers")]
        public async Task Operator3Detection(Func<string, ValueTask<Token[]>> run)
        {
            await Task.WhenAll(
                TokenFactory.OperatorChars.
                    SelectMany(ch1 => TokenFactory.OperatorChars.
                        SelectMany(ch2 => TokenFactory.OperatorChars.
                            Select(ch3 => (ch1, ch2, ch3)))).
                    Select(async entry =>
                    {
                        var text = $"123 {entry.ch1}{entry.ch2}{entry.ch3} abc";
                        var actual = await run(text);

                        Assert.AreEqual(
                            new Token[] {
                                Numeric("123"),
                                WhiteSpace(),
                                Identity($"{entry.ch1}{entry.ch2}{entry.ch3}"),
                                WhiteSpace(),
                                Identity("abc") },
                            actual);
                    }));
        }
    }
}
