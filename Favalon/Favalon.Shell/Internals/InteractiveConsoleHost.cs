﻿// This is part of Favalon project - https://github.com/kekyo/Favalon
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

        private InteractiveConsoleHost() :
            base(TextRange.Create("console", Range.MaxLength))
        {
        }

        private void PushReadKey()
        {
            var key = System.Console.ReadKey(true);

            var modifier = InteractiveModifiers.None;

            if (key.Modifiers.HasFlag(System.ConsoleModifiers.Control))
            {
                modifier |= InteractiveModifiers.Control;
            }
            if (key.Modifiers.HasFlag(System.ConsoleModifiers.Alt))
            {
                modifier |= InteractiveModifiers.Alt;
            }

            base.OnNext(new InteractiveInformation(key.KeyChar, modifier));
        }

        public void Abort() =>
            abort = true;

        public void Run()
        {
            this.Write(FeedbackLevels.Feedback, "Favalon> ");

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

        public override void Write(FeedbackLevels level, char ch) =>
            System.Console.Write(ch);

        public override void Write(FeedbackLevels level, string text) =>
            System.Console.Write(text);

        public override void WriteLine(FeedbackLevels level, string text) =>
            System.Console.WriteLine(text);

        public override void WriteLine(FeedbackLevels level) =>
            System.Console.WriteLine();

        public static InteractiveConsoleHost Create() =>
            new InteractiveConsoleHost();
    }
}
