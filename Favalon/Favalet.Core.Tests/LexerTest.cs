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

using Favalet.Tokens;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using static Favalet.Tokens.TokenFactory;

namespace Favalet
{
    [TestFixture]
    public sealed class LexerTest
    {
        private static readonly Func<string, Token[]>[] LexerRunners =
            new[]
            {
                new Func<string, Token[]>(text => Lexer.Tokenize(text).ToArray()),
                new Func<string, Token[]>(text => Lexer.Tokenize(text.AsEnumerable()).ToArray()),
                new Func<string, Token[]>(text => Lexer.Tokenize(new StringReader(text)).ToArray()),
            };

        ////////////////////////////////////////////////////

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokens(Func<string, Token[]> run)
        {
            var text = "abc def ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    WhiteSpace(),
                    Identity("def"),
                    WhiteSpace(),
                    Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensBeforeSpace(Func<string, Token[]> run)
        {
            var text = "  abc def ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    WhiteSpace(),
                    Identity("def"),
                    WhiteSpace(),
                    Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensAfterSpace(Func<string, Token[]> run)
        {
            var text = "abc def ghi  ";
            var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensLongSpace(Func<string, Token[]> run)
        {
            var text = "abc      def      ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    WhiteSpace(),
                    Identity("def"),
                    WhiteSpace(),
                    Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensBeforeBrackets(Func<string, Token[]> run)
        {
            var text = "(abc def) ghi";
            var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensAfterBrackets(Func<string, Token[]> run)
        {
            var text = "abc (def ghi)";
            var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensWithSpacingBrackets(Func<string, Token[]> run)
        {
            var text = "abc ( def ) ghi";
            var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensWithNoSpacingBrackets(Func<string, Token[]> run)
        {
            var text = "abc(def)ghi";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    Open('('),
                    Identity("def"),
                    Close(')'),
                    Identity("ghi") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTrailsNumericTokens(Func<string, Token[]> run)
        {
            var text = "a12 d34 g56";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("a12"),
                    WhiteSpace(),
                    Identity("d34"),
                    WhiteSpace(),
                    Identity("g56") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableSignLikeOperatorAndIdentityTokens1(Func<string, Token[]> run)
        {
            var text = "+abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("+"),
                    Identity("abc") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableSignLikeOperatorAndIdentityTokens2(Func<string, Token[]> run)
        {
            var text = "-abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("-"),
                    Identity("abc") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableStrictOperatorAndIdentityTokens1(Func<string, Token[]> run)
        {
            var text = "++abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("++"),
                    Identity("abc") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableStrictOperatorAndIdentityTokens2(Func<string, Token[]> run)
        {
            var text = "--abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("--"),
                    Identity("abc") },
                actual);
        }

        ///////////////////////////////////////////////

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokens(Func<string, Token[]> run)
        {
            var text = "123 456 789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    WhiteSpace(),
                    Numeric("456"),
                    WhiteSpace(),
                    Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableCombinedIdentityAndNumericTokens(Func<string, Token[]> run)
        {
            var text = "abc 456 def";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    WhiteSpace(),
                    Numeric("456"),
                    WhiteSpace(),
                    Identity("def") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensBeforeBrackets(Func<string, Token[]> run)
        {
            var text = "(123 456) 789";
            var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensAfterBrackets(Func<string, Token[]> run)
        {
            var text = "123 (456 789)";
            var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensWithSpacingBrackets(Func<string, Token[]> run)
        {
            var text = "123 ( 456 ) 789";
            var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensWithNoSpacingBrackets(Func<string, Token[]> run)
        {
            var text = "123(456)789";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    Open('('),
                    Numeric("456"),
                    Close(')'),
                    Numeric("789") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensBeforeString(Func<string, Token[]> run)
        {
            var text = "\"abc def\" 123";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc def"),
                    WhiteSpace(),
                    Numeric("123") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensAfterString(Func<string, Token[]> run)
        {
            var text = "123 \"abc def\"";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    WhiteSpace(),
                    String("abc def") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensSpacingStrings(Func<string, Token[]> run)
        {
            var text = "123 \"abc def\" 456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    WhiteSpace(),
                    String("abc def"),
                    WhiteSpace(),
                    Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableNumericTokensNoSpacingStrings(Func<string, Token[]> run)
        {
            var text = "123\"abc def\"456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    String("abc def"),
                    Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensBeforeStringWithNoSpacing(Func<string, Token[]> run)
        {
            var text = "\"abc def\"abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc def"),
                    Identity("abc") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensAfterStringWithNoSpacing(Func<string, Token[]> run)
        {
            var text = "abc\"abc def\"";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    String("abc def") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableIdentityTokensNoSpacingStrings(Func<string, Token[]> run)
        {
            var text = "abc\"abc def\"def";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    String("abc def"),
                    Identity("def") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableStringTokensNoSpacingOperators(Func<string, Token[]> run)
        {
            var text = "+\"abc def\"-";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("+"),
                    String("abc def"),
                    Identity("-") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableOperatorTokensNoSpacingStrings(Func<string, Token[]> run)
        {
            var text = "\"abc\"+\"def\"";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc"),
                    Identity("+"),
                    String("def") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableStringTokensNoSpacingBrackets(Func<string, Token[]> run)
        {
            var text = "(\"abc def\")";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Open('('),
                    String("abc def"),
                    Close(')') },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableOpenBracketTokensNoSpacingStrings(Func<string, Token[]> run)
        {
            var text = "\"abc\"(\"def\"";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc"),
                    Open('('),
                    String("def") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableCloseBracketTokensNoSpacingStrings(Func<string, Token[]> run)
        {
            var text = "\"abc\")\"def\"";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc"),
                    Close(')'),
                    String("def") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableContinuousStrings(Func<string, Token[]> run)
        {
            var text = "\"abc\"\"def\"";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    String("abc"),
                    String("def") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerablePlusSignNumericTokens(Func<string, Token[]> run)
        {
            var text = "+123 +456 +789";
            var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void EnumerableMinusSignNumericTokens(Func<string, Token[]> run)
        {
            var text = "-123 -456 -789";
            var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void EnumerablePlusOperatorNumericTokens(Func<string, Token[]> run)
        {
            var text = "+ 123 + 456 + 789";
            var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void EnumerableMinusOperatorNumericTokens(Func<string, Token[]> run)
        {
            var text = "- 123 - 456 - 789";
            var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void EnumerablePlusOperatorWithSpaceAndNumericTokens(Func<string, Token[]> run)
        {
            var text = "123 + 456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    WhiteSpace(),
                    Identity("+"),
                    WhiteSpace(),
                    Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableMinusOperatorWithSpaceAndNumericTokens(Func<string, Token[]> run)
        {
            var text = "123 - 456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    WhiteSpace(),
                    Identity("-"),
                    WhiteSpace(),
                    Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerablePlusOperatorSideBySideAndNumericTokens2(Func<string, Token[]> run)
        {
            var text = "123+456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    PlusSign(),
                    Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableMinusOperatorSideBySideAndNumericTokens(Func<string, Token[]> run)
        {
            var text = "123-456";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    MinusSign(),
                    Numeric("456") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableComplexNumericOperatorTokens1(Func<string, Token[]> run)
        {
            var text = "-123*(+456+789)";
            var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void EnumerableComplexNumericOperatorTokens2(Func<string, Token[]> run)
        {
            var text = "+123*(-456-789)";
            var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void EnumerableStrictOperatorAndNumericTokens1(Func<string, Token[]> run)
        {
            var text = "++123";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("++"),
                    Numeric("123") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableStrictOperatorAndNumericTokens2(Func<string, Token[]> run)
        {
            var text = "--123";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("--"),
                    Numeric("123") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableCombineIdentityAndNumericTokensWithOperator(Func<string, Token[]> run)
        {
            var text = "abc+123";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Identity("abc"),
                    PlusSign(),
                    Numeric("123") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void EnumerableCombineNumericAndIdentityTokensWithOperator(Func<string, Token[]> run)
        {
            var text = "123+abc";
            var actual = run(text);

            Assert.AreEqual(
                new Token[] {
                    Numeric("123"),
                    Identity("+"),
                    Identity("abc") },
                actual);
        }

        [TestCaseSource("LexerRunners")]
        public void Operator1Detection(Func<string, Token[]> run)
        {
            foreach (var ch in TokenFactory.OperatorChars)
            {
                var text = $"123 {ch} abc";
                var actual = run(text);

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

        [TestCaseSource("LexerRunners")]
        public void Operator2Detection(Func<string, Token[]> run)
        {
            Parallel.ForEach(
                TokenFactory.OperatorChars.
                    SelectMany(ch1 => TokenFactory.OperatorChars.
                        Select(ch2 => (ch1, ch2))),
                entry =>
                {
                    var text = $"123 {entry.ch1}{entry.ch2} abc";
                    var actual = run(text);

                    Assert.AreEqual(
                        new Token[] {
                            Numeric("123"),
                            WhiteSpace(),
                            Identity($"{entry.ch1}{entry.ch2}"),
                            WhiteSpace(),
                            Identity("abc") },
                        actual);
                });
        }

        [TestCaseSource("LexerRunners")]
        public void Operator3Detection(Func<string, Token[]> run)
        {
            Parallel.ForEach(
                TokenFactory.OperatorChars.
                    SelectMany(ch1 => TokenFactory.OperatorChars.
                        SelectMany(ch2 => TokenFactory.OperatorChars.
                            Select(ch3 => (ch1, ch2, ch3)))),
                entry =>
                {
                    var text = $"123 {entry.ch1}{entry.ch2}{entry.ch3} abc";
                    var actual = run(text);

                    Assert.AreEqual(
                        new Token[] {
                            Numeric("123"),
                            WhiteSpace(),
                            Identity($"{entry.ch1}{entry.ch2}{entry.ch3}"),
                            WhiteSpace(),
                            Identity("abc") },
                        actual);
                });
        }
    }
}
