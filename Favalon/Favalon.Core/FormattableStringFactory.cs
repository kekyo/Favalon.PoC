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

namespace System.Runtime.CompilerServices
{
#if NET40 || NET45 || NETSTANDARD1_0
    public static class FormattableStringFactory
    {
        private sealed class ConcreteFormattableString : FormattableString
        {
            private readonly object[] arguments;

            public ConcreteFormattableString(string format, object[] arguments)
            {
                this.Format = format;
                this.arguments = arguments;
            }

            public override string Format { get; }

            public override int ArgumentCount => arguments.Length;

            public override object GetArgument(int index) => arguments[index];

            public override object[] GetArguments() => arguments;

            public override string ToString(IFormatProvider formatProvider) =>
                string.Format(formatProvider, this.Format, arguments);
        }

        public static FormattableString Create(string format, params object[] arguments) =>
            new ConcreteFormattableString(format, arguments);
    }
#endif
}
