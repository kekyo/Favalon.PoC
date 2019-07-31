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

namespace Favalon.IO
{
    public enum LogLevels
    {
        Information,
        Warning,
        Error
    }

    public interface IInteractiveOutput
    {
        void Echo(char ch);
        void Echo(string text);
        void EndOfLine();
        void Backspace();
        void Delete();

        void Write(string text);
        void WriteLine();
        void WriteLine(string text);

        void WriteLog(LogLevels level, string text);
    }
}
