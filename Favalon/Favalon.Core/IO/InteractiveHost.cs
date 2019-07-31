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

using System;

namespace Favalon.IO
{
    public abstract class InteractiveHost :
        ObservableBase<InteractiveInformation>, IInteractiveOutput
    {
        protected InteractiveHost()
        { }

        public abstract void Echo(char ch);

        public virtual void Echo(string text)
        {
            foreach (var ch in text)
            {
                this.Echo(ch);
            }
        }

        public virtual void EndOfLine() =>
            this.Echo(Environment.NewLine);

        public virtual void Backspace() =>
            this.Echo('\u0008');

        public virtual void Delete() =>
            this.Echo('\u007f');

        public abstract void Write(string text);

        public virtual void WriteLine() =>
            this.Write(Environment.NewLine);

        public virtual void WriteLine(string text)
        {
            this.Write(text);
            this.Write(Environment.NewLine);
        }

        public virtual void WriteLog(LogLevels level, string text)
        {
            this.Echo(text);
            this.Echo(Environment.NewLine);
        }
    }
}
