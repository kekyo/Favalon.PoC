using System;

namespace TypeInference.Types
{
    public sealed class CLRType : AvalonType
    {
        private readonly Type type;

        internal CLRType(Type type) => this.type = type;

        public Type RawType => this.type;

        public override int GetHashCode() =>
            this.type.GetHashCode();

        public override bool Equals(AvalonType other) =>
            this.type == (other as CLRType)?.type;
    }
}
