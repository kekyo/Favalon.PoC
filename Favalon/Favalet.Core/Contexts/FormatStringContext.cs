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

using System.Collections.Generic;
using Favalet.Expressions;

namespace Favalet.Contexts
{
    public interface IFormatStringContext
    {
        string GetPlaceholderIndexString(int index);

        string Format(IExpression expression, params object[] args);

        IFormatStringContext SuppressRecursive();
    }

    public abstract class FormatStringContext : IFormatStringContext
    {
        private readonly Dictionary<int, int>? relativeIndexes;

        protected FormatStringContext(bool useRelativeIndex) =>
            this.relativeIndexes = useRelativeIndex ? new Dictionary<int, int>() : null;

        protected FormatStringContext(FormatStringContext parent) =>
            this.relativeIndexes = parent.relativeIndexes;

        public string GetPlaceholderIndexString(int index)
        {
            if (this.relativeIndexes is Dictionary<int, int> relativeIndexes)
            {
                if (!relativeIndexes.TryGetValue(index, out var relativeIndex))
                {
                    relativeIndex = relativeIndexes.Count;
                    relativeIndexes.Add(index, relativeIndex);
                }
                return relativeIndex.ToString();
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
