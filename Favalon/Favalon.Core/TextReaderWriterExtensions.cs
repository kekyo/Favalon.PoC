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
using System.IO;
using System.Threading.Tasks;

namespace Favalon
{
    public static class TextReaderWriterExtensions
    {
#if true
        public static Task<string> InputAsync(this TextReader tr) =>
            Task.Factory.StartNew(() => tr.ReadLine());

        public static Task PrintAsync(this TextWriter tw, FormattableString text) =>
            Task.Factory.StartNew(() => tw.WriteLine(text.Format, text.GetArguments()));
#else
        public static Task<string> InputAsync(this TextReader tr) =>
            tr.ReadLineAsync();

        public static Task PrintAsync(this TextWriter tw, FormattableString text) =>
            tw.WriteLineAsync(text.Format, text.GetArguments());
#endif
    }
}
