using System;

namespace TypeInference
{
    public sealed class Option
    {
        private Option() { }

        public static Option None = new Option();

        public static Option<T> Some<T>(T value) =>
            new Option<T>(value);
    }

    public struct Option<T>
    {
        private readonly bool some;
        private readonly T value;

        public Option(T value)
        {
            this.value = value;
            this.some = true;
        }

        public static Option<T> None =>
            default;

        public bool IsSome() =>
            this.some;

        public T Value =>
            this.some ? this.value : throw new InvalidOperationException();

        public U Select<U>(Func<T, U> mapper) =>
            this.some ? mapper(this.value) : default;

        public Option<U> SelectMany<U>(Func<T, Option<U>> binder) =>
            this.some ? binder(this.value) : default;

        public bool TryGetValue(out T value)
        {
            if (this.some)
            {
                value = this.value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public static implicit operator Option<T>(Option none) =>
            default;
        public static implicit operator Option<T>(T value) =>
            new Option<T>(value);
    }
}
