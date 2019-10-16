using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Favalon.Expressions
{
    public sealed class Null : Value
    {
        private Null()
        {
        }

        public override Expression HigherOrder =>
            null!;

        public override bool Equals(Value? other) =>
            other is Null;

        public override Expression Run() =>
            this;

        internal static readonly Null Instance = new Null();
    }
}
