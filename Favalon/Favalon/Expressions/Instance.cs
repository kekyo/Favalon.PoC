using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Favalon.Expressions
{
    public class Instance : Value, IEquatable<Instance?>
    {
        public readonly object Value;

        internal Instance(object value, Expression higherOrder)
        {
            this.Value = value;
            this.HigherOrder = higherOrder;
        }

        public override Expression HigherOrder { get; }

        public override int GetHashCode() =>
            this.Value?.GetHashCode() ?? 0;

        public bool Equals(Instance? other) =>
            other?.Value?.Equals(this.Value) ?? false;

        public override bool Equals(Value? other) =>
            this.Equals(other as Instance);

        public override string ToString() =>
            this.Value?.ToString() ?? "(null)";

        public override Expression Run() =>
            this;
    }

    public sealed class Instance<T> : Instance
    {
        private static readonly Expression higherOrder =
            Factories.FromType<T>();

        internal Instance(T value) :
            base(value!, higherOrder)
        {
        }

        public new T Value =>
            (T)base.Value;

        public override Expression Run() =>
            this;
    }
}
