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

using Favalet;
using Favalon.IO;
using System.Collections.Generic;
using System.Text;

namespace Favalon.Internals
{
    public sealed class InteractiveConsoleHost : InteractiveHost
    {
        private readonly StringBuilder lineBuffer = new StringBuilder();
        private readonly List<Position> cursorPositions = new List<Position>();
        private int position;
        private volatile bool abort;

        private InteractiveConsoleHost()
        {
        }

        private void PushReadKey()
        {
            var key = System.Console.ReadKey(true);

            var keyChar = key.KeyChar;
            var modifier = InteractiveModifiers.None;

            switch (key.Key)
            {
                case System.ConsoleKey.Delete:
                    keyChar = '\x007f';
                    break;
                case System.ConsoleKey.UpArrow:
                    keyChar = '\x0010';
                    modifier |= InteractiveModifiers.Control;
                    break;
                case System.ConsoleKey.DownArrow:
                    keyChar = '\x000e';
                    modifier |= InteractiveModifiers.Control;
                    break;
                case System.ConsoleKey.LeftArrow:
                    keyChar = '\x0006';
                    modifier |= InteractiveModifiers.Control;
                    break;
                case System.ConsoleKey.RightArrow:
                    keyChar = '\x0002';
                    modifier |= InteractiveModifiers.Control;
                    break;
                default:
                    if (key.Modifiers.HasFlag(System.ConsoleModifiers.Control))
                    {
                        modifier |= InteractiveModifiers.Control;
                    }
                    if (key.Modifiers.HasFlag(System.ConsoleModifiers.Alt))
                    {
                        modifier |= InteractiveModifiers.Alt;
                    }
                    break;
            }

            base.OnNext(InteractiveInformation.Create(keyChar, modifier));
        }

        private static Position GetCurrentPosition() =>
            Position.Create(System.Console.CursorTop, System.Console.CursorLeft);

        public void Abort() =>
            abort = true;

        public void Run()
        {
            try
            {
                while (!abort)
                {
                    this.PushReadKey();
                }
            }
            finally
            {
                base.OnCompleted();
            }
        }

        public override void Echo(char ch)
        {
            if (cursorPositions.Count == 0)
            {
                cursorPositions.Add(GetCurrentPosition());
            }

            System.Console.Write(ch);
            lineBuffer.Append(ch);
            cursorPositions.Insert(position++ + 1, GetCurrentPosition());
        }

        public override void EndOfLine()
        {
            System.Console.WriteLine();
            lineBuffer.Clear();
            cursorPositions.Clear();
            position = 0;
        }

        public override void Backspace()
        {
            if (position >= 1)
            {
                var lastPosition = cursorPositions[position];
                position--;
                var newPosition = cursorPositions[position];

                var differ = lastPosition.Column - newPosition.Column;

                lineBuffer.Remove(position, 1);

                for (var index = position; index < cursorPositions.Count - 1; index++)
                {
                    var fromPosition = cursorPositions[index + 1];
                    cursorPositions[index] = Position.Create(fromPosition.Line, fromPosition.Column - differ);
                }

                System.Console.SetCursorPosition(newPosition.Column, newPosition.Line);
                System.Console.Write(lineBuffer.ToString().Substring(position) + new string(' ', differ));
                System.Console.SetCursorPosition(newPosition.Column, newPosition.Line);

                cursorPositions.RemoveAt(position + 1);
            }
        }

        public override void Delete()
        {
            if (position >= 1)
            {
                var currentPosition = cursorPositions[position];
                var previousPosition = cursorPositions[position - 1];

                var differ = currentPosition.Column - previousPosition.Column;

                lineBuffer.Remove(position - 1, 1);

                for (var index = position; index < cursorPositions.Count - 1; index++)
                {
                    var fromPosition = cursorPositions[index + 1];
                    cursorPositions[index] = Position.Create(fromPosition.Line, fromPosition.Column - differ);
                }

                System.Console.SetCursorPosition(currentPosition.Column, currentPosition.Line);
                System.Console.Write(lineBuffer.ToString().Substring(position) + new string(' ', differ));
                System.Console.SetCursorPosition(currentPosition.Column, currentPosition.Line);

                cursorPositions.RemoveAt(position + 1);
            }
        }

        private void EndOfLineIfRequired()
        {
            if (cursorPositions.Count >= 1)
            {
                this.EndOfLine();
            }
        }

        public override void Write(string text)
        {
            this.EndOfLineIfRequired();
            System.Console.Write(text);
        }

        public override void WriteLine()
        {
            this.EndOfLineIfRequired();
            System.Console.WriteLine();
        }

        public override void WriteLine(string text)
        {
            this.EndOfLineIfRequired();
            System.Console.WriteLine(text);
        }

        public override void WriteLog(LogLevels level, string text)
        {
            this.EndOfLineIfRequired();

            var fc = System.Console.ForegroundColor;
            switch (level)
            {
                case LogLevels.Warning:
                    System.Console.ForegroundColor = System.ConsoleColor.Yellow;
                    System.Console.WriteLine(text);
                    System.Console.ForegroundColor = fc;
                    break;
                case LogLevels.Error:
                    System.Console.ForegroundColor = System.ConsoleColor.Red;
                    System.Console.WriteLine(text);
                    System.Console.ForegroundColor = fc;
                    break;
                default:
                    System.Console.WriteLine(text);
                    break;
            }
        }

        public static InteractiveConsoleHost Create() =>
            new InteractiveConsoleHost();
    }
}
