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

namespace Favalon.Internals
{
    public sealed class InteractiveConsoleHost : InteractiveHost
    {
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

        public override void Write(char ch) =>
            System.Console.Write(ch);

        public override void Write(string text) =>
            System.Console.Write(text);

        public override void WriteLine() =>
            System.Console.WriteLine();

        public override void WriteLog(LogLevels level, string text)
        {
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
