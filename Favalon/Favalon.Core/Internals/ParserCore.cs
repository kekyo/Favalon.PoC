using System;
using System.Diagnostics;
using System.Text;

namespace Favalon.Internals
{
    internal sealed class ParserCore
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
        public static readonly string operatorChars = new string(new[]
{
            '!'/* , '"' */, '#', '$', '%', '&' /* , ''', '(', ')' */,
            '*', '+', ',', '-', '.', '/', ':', ';', '<', '=', '>', '?',
            '@', '[', '\\', ']', '^', '_', '`', '{', '|', '}', '~'
        });

        private States state = States.Idle;
        private readonly StringBuilder buffer = new StringBuilder();

        private static bool IsOperator(char inch) =>
            operatorChars.IndexOf(inch) >= 0;
        private static bool IsFlushing(char inch) =>
            inch == flushingChar;

        public Token? Examine(char inch)
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
                        var token = new Token(TokenTypes.Variable, buffer.ToString());
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

        public Token? Flush() =>
            this.Examine(flushingChar);
    }
}
