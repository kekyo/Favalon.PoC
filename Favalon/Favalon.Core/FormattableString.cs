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

using System.Globalization;

namespace System
{
#if NET40 || NET45 || NETSTANDARD1_0
    public abstract class FormattableString : IFormattable
    {
        protected FormattableString()
        { }

        public abstract string Format { get; }
        public abstract int ArgumentCount { get; }

        public static string Invariant(FormattableString formattable) =>
            formattable.ToString(CultureInfo.InvariantCulture);
        public abstract object GetArgument(int index);
        public abstract object[] GetArguments();
        public abstract string ToString(IFormatProvider formatProvider);
        public override string ToString() =>
            this.ToString(CultureInfo.InvariantCulture);
        public string ToString(string format, IFormatProvider formatProvider) =>
            this.ToString(formatProvider);
    }
#endif
}
