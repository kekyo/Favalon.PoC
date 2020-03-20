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


// Non-nullable field is uninitialized. Consider declaring as nullable.
#pragma warning disable CS8618

using System.Runtime.CompilerServices;

namespace System
{
    internal static class Lazy
    {
#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static Lazy<T> Create<T>(Func<T> generator) =>
            new Lazy<T>(generator);
    }

#if NET35
    internal sealed class Lazy<T>
    {
        private Func<T>? generator;
        private T value;

        public Lazy(Func<T> generator) =>
            this.generator = generator;

        public T Value
        {
            get
            {
                if (this.generator != null)
                {
                    lock (this)
                    {
                        if (this.generator != null)
                        {
                            this.value = this.generator();
                            this.generator = null;
                        }
                    }
                }

                return value;
            }
        }
    }
#endif
}
