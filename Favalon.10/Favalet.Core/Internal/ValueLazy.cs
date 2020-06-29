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

// A default expression introduces a null value for a type parameter.
#pragma warning disable CS8653

using System;
using System.Runtime.CompilerServices;

namespace Favalet.Internal
{
    internal static class ValueLazy
    {
#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ValueLazy<T> Create<T>(Func<T> generator) =>
            new ValueLazy<T>(generator);

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ValueLazy<T, U> Create<T, U>(T argument, Func<T, U> generator) =>
            new ValueLazy<T, U>(argument, generator);
    }

    internal struct ValueLazy<T>
    {
        private Func<T>? generator;
        private T value;

        public ValueLazy(Func<T> generator)
        {
            this.generator = generator;
            this.value = default;
        }

        public T Value
        {
            get
            {
                if (this.generator != null)
                {
                    this.value = this.generator();
                    this.generator = null;
                }

                return value;
            }
        }

        public override string? ToString() =>
            (this.generator != null) ? "(Not generated)" : this.value?.ToString();
    }

    internal struct ValueLazy<T, U>
    {
        private Func<T, U>? generator;
        private T argument;
        private U value;

        public ValueLazy(T argument, Func<T, U> generator)
        {
            this.generator = generator;
            this.argument = argument;
            this.value = default;
        }

        public U Value
        {
            get
            {
                if (this.generator != null)
                {
                    this.value = this.generator(this.argument);
                    this.generator = null;
                    this.argument = default;
                }

                return value;
            }
        }

        public override string? ToString() =>
            (this.generator != null) ? "(Not generated)" : this.value?.ToString();
    }
}
