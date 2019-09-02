using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Favalon
{
    public sealed class Parser
    {
        private enum States
        {
            Idle,
            Numeric,
            Operator,
            String,
            Variable,
        }

        private const char flushingChar = '\xffff';

        public static readonly string OperatorChars = new string(new[]
        {
            '!'/* , '"' */, '#', '$', '%', '&' /* , ''', '(', ')' */,
            '*', '+', ',', '-', '.', '/', ':', ';', '<', '=', '>', '?',
            '@', '[', '\\', ']', '^', '_', '`', '{', '|', '}', '~'
        });

        private States state = States.Idle;
        private readonly StringBuilder buffer = new StringBuilder();

        private static bool IsOperator(char inch) =>
            OperatorChars.IndexOf(inch) >= 0;
        private static bool IsFlushing(char inch) =>
            inch == flushingChar;

        private Token? Examine(char inch)
        {
            Token? result = null;

            while (true)
            {
                if (state == States.Idle)
                {
                    if (IsFlushing(inch))
                    {
                        return result;
                    }
                    else if (inch == '"')
                    {
                        state = States.String;
                        return result;
                    }
                    else if (char.IsNumber(inch))
                    {
                        state = States.Numeric;
                    }
                    else if (IsOperator(inch))
                    {
                        state = States.Operator;
                    }
                    else if (!char.IsWhiteSpace(inch))
                    {
                        state = States.Variable;
                    }
                    else
                    {
                        return result;
                    }
                }

                if (state == States.Numeric)
                {
                    if (char.IsNumber(inch))
                    {
                        buffer.Append(inch);
                        return result;
                    }
                    else
                    {
                        var token = new Token(TokenTypes.Numeric, buffer.ToString());
                        buffer.Clear();

                        Debug.Assert(!result.HasValue);
                        result = token;

                        state = States.Idle;
                        continue;
                    }
                }
                else if (state == States.Operator)
                {
                    if (IsOperator(inch))
                    {
                        buffer.Append(inch);
                        return result;
                    }
                    else
                    {
                        var token = new Token(TokenTypes.Operator, buffer.ToString());
                        buffer.Clear();

                        Debug.Assert(!result.HasValue);
                        result = token;

                        state = States.Idle;
                        continue;
                    }
                }
                else if (state == States.String)
                {
                    if (IsFlushing(inch))
                    {
                        throw new Exception();
                    }
                    else if (inch != '"')
                    {
                        buffer.Append(inch);
                        Debug.Assert(!result.HasValue);
                        return null;
                    }
                    else
                    {
                        var token = new Token(TokenTypes.String, buffer.ToString());
                        buffer.Clear();

                        Debug.Assert(!result.HasValue);
                        result = token;

                        state = States.Idle;

                        return result;
                    }
                }
                else
                {
                    Debug.Assert(state == States.Variable);

                    if (!char.IsWhiteSpace(inch) && !IsOperator(inch) && (inch != '"') && !IsFlushing(inch))
                    {
                        buffer.Append(inch);
                        return result;
                    }
                    else
                    {
                        var token = new Token(TokenTypes.Variable, buffer.ToString());
                        buffer.Clear();

                        Debug.Assert(!result.HasValue);
                        result = token;

                        state = States.Idle;
                        continue;
                    }
                }
            }
        }

        public IEnumerable<Token> Tokenize(string line, bool withFlush = true)
        {
            var index = 0;
            while (index < line.Length)
            {
                var inch = line[index++];
                var token = this.Examine(inch);
                if (token is Token t)
                {
                    yield return t;
                }
            }

            if (withFlush)
            {
                if (this.Examine(flushingChar) is Token token)
                {
                    yield return token;
                }
            }
        }

        public Token? Flush() =>
            this.Examine(flushingChar);
    }
}
