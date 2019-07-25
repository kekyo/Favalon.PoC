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

using Favalon.Win32;
using System;

namespace Favalon
{
    internal struct KeyInformation
    {
        public readonly VirtualKeys VirtualKey;
        public readonly char UnicodeChar;

        internal KeyInformation(VirtualKeys virtualKey, char unicodeChar)
        {
            this.VirtualKey = virtualKey;
            this.UnicodeChar = unicodeChar;
        }
    }

    internal sealed class ConsoleReader : IObservable<KeyInformation>
    {
        private volatile IObserver<KeyInformation>? observer;

        public void Run()
        {
            var stdin = NativeMethods.GetStdHandle(NativeMethods.STD_INPUT_HANDLE);

            while (true)
            {
                if (NativeMethods.ReadConsoleInput(stdin) is NativeMethods.KEY_EVENT_RECORD record)
                {
                    if (record.bKeyDown)
                    {
                        observer?.OnNext(new KeyInformation(record.wVirtualKeyCode, record.UnicodeChar));
                    }
                }
            }
        }

        public IDisposable Subscribe(IObserver<KeyInformation> observer)
        {
            this.observer = observer;
            return null!;
        }

        private sealed class Disposer : IDisposable
        {
            private readonly ConsoleReader parent;

            public Disposer(ConsoleReader parent) =>
                this.parent = parent;

            public void Dispose() =>
                parent.observer = null;
        }
    }
}
