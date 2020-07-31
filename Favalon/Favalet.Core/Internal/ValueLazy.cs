using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

#pragma warning disable CS8601
#pragma warning disable CS8618
#pragma warning disable CS8653

namespace Favalet.Internal
{
    internal static class ValueLazy
    {
#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [DebuggerStepThrough]
        public static ValueLazy<T> Create<T>(Func<T> generator) =>
            new ValueLazy<T>(generator);

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        [DebuggerStepThrough]
        public static ValueLazy<T, U> Create<T, U>(T argument, Func<T, U> generator) =>
            new ValueLazy<T, U>(argument, generator);
    }

    [DebuggerStepThrough]
    internal struct ValueLazy<T>
    {
        private Func<T>? generator;
        private T value;

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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

    [DebuggerStepThrough]
    internal struct ValueLazy<T, U>
    {
        private Func<T, U>? generator;
        private T argument;
        private U value;

#if !NET35 && !NET40
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
