////////////////////////////////////////////////////////////////////////////
//
// Favalon - An Interactive Shell Based on a Typed Lambda Calculus.
// Copyright (c) 2018-2020 Kouji Matsui (@kozy_kekyo, @kekyo2)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//	http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Favalet.Expressions;
using Favalet.Internal;

namespace Favalet.Contexts
{
    [Flags]
    public enum FormatStringOptions
    {
        Default = 0x00,
        UseRelativeIndex = 0x01,
        UseSimpleLabels = 0x02
    }

    public interface IFormatStringContext
    {
        bool UseSimpleLabel { get; }

        string GetPlaceholderIndexString(int index);

        string Format(IExpression expression, params object[] args);

        IFormatStringContext SuppressRecursive();
    }

    public abstract class FormatStringContext : IFormatStringContext
    {
        private static readonly char[] alphabets =
            Enumerable.Range('a', 'z' - 'a' + 1).Select(i => (char)i).ToArray();

        private readonly Dictionary<int, int>? relativeIndexes;

        protected FormatStringContext(FormatStringOptions options)
        {
            this.UseSimpleLabel =
                (options & FormatStringOptions.UseSimpleLabels) == FormatStringOptions.UseSimpleLabels;
            if ((options & FormatStringOptions.UseRelativeIndex) == FormatStringOptions.UseRelativeIndex)
            {
                this.relativeIndexes = new Dictionary<int, int>();
            }
        }

        protected FormatStringContext(FormatStringContext parent)
        {
            this.UseSimpleLabel = parent.UseSimpleLabel;
            this.relativeIndexes = parent.relativeIndexes;
        }

        public bool UseSimpleLabel { get; }

        public string GetPlaceholderIndexString(int index)
        {
            if (this.relativeIndexes is Dictionary<int, int> relativeIndexes)
            {
                if (!relativeIndexes.TryGetValue(index, out var relativeIndex))
                {
                    relativeIndex = relativeIndexes.Count;
                    relativeIndexes.Add(index, relativeIndex);
                }

                var sb = new StringBuilder();
                while (true)
                {
                    var i = relativeIndex % alphabets.Length;
                    var ch = alphabets[i];
                    sb.Insert(0, ch);

                    if (relativeIndex == 0)
                    {
                        return sb.ToString();
                    }

                    relativeIndex = relativeIndex / alphabets.Length;
                }
            }
            else
            {
                return index.ToString();
            }
        }

        public abstract string Format(IExpression expression, params object[] args);

        public abstract IFormatStringContext SuppressRecursive();
    }
}
